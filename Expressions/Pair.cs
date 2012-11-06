namespace Expressions
{
    public struct Pair<T, U>
    {
        public readonly T Fst;
        public readonly U Snd;

        public Pair(T fst, U snd)
        {
            this.Fst = fst;
            this.Snd = snd;
        }
    }
}