namespace Library.Elasticsearch.Entities
{
    public class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.ToLocalTime();
    }
}
