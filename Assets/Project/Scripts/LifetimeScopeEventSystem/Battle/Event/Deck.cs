using System.Collections.Generic;

namespace LSES.Battle.Event
{
    public class DeckDrawEventBundle
    {
        public EventBus<DeckDrawRequestEvent> Request { get; }
        public EventBus<DeckDrawResponseEvent> Response { get; }
        public DeckDrawEventBundle(EventBus<DeckDrawRequestEvent> request, EventBus<DeckDrawResponseEvent> response)
        {
            Request = request;
            Response = response;
        }
    }

    public record DeckDrawRequestEvent(int Count) : EventObject;
    public record DeckDrawResponseEvent(List<SkillData> DrawCard) : EventObject;
    public record DeckGetEvent(DeckList List) : EventObject;
}