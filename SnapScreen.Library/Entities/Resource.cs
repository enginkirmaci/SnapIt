namespace SnapScreen.Library.Entities
{
    public class Resource<T>
    {
        public T Key { get; set; }
        public string Text { get; set; }

        public Resource(T key, string text)
        {
            Key = key;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}