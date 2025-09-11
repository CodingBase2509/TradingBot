using Trading.Core.Domain;

namespace Trading.Foundation.Buffers;

public sealed class CandleRingBuffer(int capacity) : RingBuffer<Candle>(capacity)
{
    
}