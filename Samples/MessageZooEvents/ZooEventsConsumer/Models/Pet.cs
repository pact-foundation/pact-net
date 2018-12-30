namespace ZooEventsConsumer.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PetType Type { get; set; }

        protected bool Equals(Pet other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Type;
                return hashCode;
            }
        }
    }
}