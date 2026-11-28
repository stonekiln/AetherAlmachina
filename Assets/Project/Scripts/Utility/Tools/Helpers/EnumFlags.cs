using System;
using System.Runtime.InteropServices;

namespace Tools.Helpers
{
    /// <summary>
    /// [Flags] 列挙型の現在値を保持します。変更操作は、値が変わったときだけ true を返します。
    /// </summary>
    public sealed class EnumFlags<TEnum> where TEnum : unmanaged, Enum
    {
        static readonly bool IsFlagsEnum = typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);
        ulong bits;

        public EnumFlags(TEnum initialValue = default)
        {
            if (!IsFlagsEnum)
            {
                throw new ArgumentException($"{typeof(TEnum).Name} には [Flags] が必要です。");
            }

            bits = ToBits(initialValue);
        }

        public TEnum Value => FromBits(bits);

        public bool IsEmpty => bits == 0;

        /// <summary>標準の Enum.HasFlag と同じく、指定したビットをすべて含むかを判定します。</summary>
        public bool HasFlag(TEnum flag) => HasAll(flag);

        /// <summary>すべてのビットが立っていれば true。指定ビットがゼロなら常に true です。</summary>
        public bool HasAll(TEnum first, params TEnum[] additional)
        {
            ulong mask = Combine(first, additional);
            return (bits & mask) == mask;
        }

        /// <summary>いずれかのビットが立っていれば true。指定ビットがゼロなら常に false です。</summary>
        public bool HasAny(TEnum first, params TEnum[] additional)
        {
            ulong mask = Combine(first, additional);
            return (bits & mask) != 0;
        }

        /// <summary>指定値と現在値で立っている状態が異なるフラグを返します。</summary>
        public TEnum GetChangedFlags(TEnum other)
            => FromBits(bits ^ ToBits(other));

        public bool Add(TEnum first, params TEnum[] additional)
            => Update(bits | Combine(first, additional));

        public bool Remove(TEnum first, params TEnum[] additional)
            => Update(bits & ~Combine(first, additional));

        /// <summary>指定したビットだけを enabled の状態に揃えます。</summary>
        public bool SetEnabled(TEnum flags, bool enabled)
            => enabled ? Add(flags) : Remove(flags);

        /// <summary>現在値全体を指定値に置き換えます。</summary>
        public bool Replace(TEnum value) => Update(ToBits(value));

        public bool Clear() => Update(0);

        bool Update(ulong next)
        {
            if (bits == next)
            {
                return false;
            }

            bits = next;
            return true;
        }

        static ulong Combine(TEnum first, TEnum[] additional)
        {
            if (additional == null)
            {
                throw new ArgumentNullException(nameof(additional));
            }

            ulong mask = ToBits(first);
            foreach (TEnum flag in additional)
            {
                mask |= ToBits(flag);
            }

            return mask;
        }

        static TEnum FromBits(ulong value)
        {
            ReadOnlySpan<ulong> storage = stackalloc ulong[] { value };
            return MemoryMarshal.Cast<ulong, TEnum>(storage)[0];
        }

        // 8 バイトをゼロで初期化し、先頭に列挙値のビット列をそのまま格納します。
        // Value で同じ領域を TEnum として読むため、符号付きの最上位ビットも保持できます。
        static ulong ToBits(TEnum value)
        {
            Span<ulong> storage = stackalloc ulong[1];
            storage[0] = 0;
            MemoryMarshal.Cast<ulong, TEnum>(storage)[0] = value;
            return storage[0];
        }
    }

    // 既存の HasAll(default) / HasAny(default) が列挙値を受け取るよう、集合版は拡張メソッドにします。
    public static class EnumFlagsExtensions
    {
        /// <summary>相手の集合に含まれるフラグをすべて持つか判定します。</summary>
        public static bool HasAll<TEnum>(this EnumFlags<TEnum> flags, EnumFlags<TEnum> other)
            where TEnum : unmanaged, Enum
            => flags.HasAll((other ?? throw new ArgumentNullException(nameof(other))).Value);

        /// <summary>相手の集合と重なるフラグが一つでもあるか判定します。</summary>
        public static bool HasAny<TEnum>(this EnumFlags<TEnum> flags, EnumFlags<TEnum> other)
            where TEnum : unmanaged, Enum
            => flags.HasAny((other ?? throw new ArgumentNullException(nameof(other))).Value);

        /// <summary>二つの集合で立っている状態が異なるフラグを返します。</summary>
        public static TEnum GetChangedFlags<TEnum>(this EnumFlags<TEnum> flags, EnumFlags<TEnum> other)
            where TEnum : unmanaged, Enum
            => flags.GetChangedFlags((other ?? throw new ArgumentNullException(nameof(other))).Value);
    }
}
