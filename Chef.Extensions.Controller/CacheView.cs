namespace Chef.Extensions.Controller
{
    internal class CacheView
    {
        public CacheView(string output)
        {
            this.Output = output;
            this.Checksum = MD5.Hash(output);
        }

        public string Output { get; }

        public string Checksum { get; }
    }
}