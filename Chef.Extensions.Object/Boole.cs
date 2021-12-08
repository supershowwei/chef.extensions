namespace Chef.Extensions.Object
{
    public struct Boole<T>
    {
        public Boole(T logic, bool value)
        {
            this.Logic = logic;
            this.Value = value;
        }

        public T Logic { get; }

        public bool Value { get; }

        public static implicit operator bool(Boole<T> boole)
        {
            return boole.Value;
        }

        public static explicit operator T(Boole<T> boole)
        {
            return boole.Logic;
        }
    }
}