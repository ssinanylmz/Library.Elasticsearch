namespace Library.Elasticsearch.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
