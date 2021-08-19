namespace Validation
{
    public class Product
    {
        public string Name { get; }
        public string Type { get; set; }
        public ProductFamily Family { get; set; }
        public ProductRange Range { get; set; }
        public double Weight { get; set; }

        public Product(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "Product{" +
                   "name='" + Name + '\'' +
                   ", type='" + Type + '\'' +
                   ", weight=" + Weight +
                   ", family=" + Family +
                   ", range=" + Range +
                   '}';
        }
    }

    public class Eyeshadow : Product
    {
        public Eyeshadow(string name) : base(name)
        {
            Type = "Eyeshadow";
            Family = ProductFamily.EYES;
        }
    }

    public class Mascara : Product
    {
        public Mascara(string name) : base(name)
        {
            Type = "Mascara";
            Family = ProductFamily.LASHES;
        }
    }

    public class Lipstick : Product
    {
        public Lipstick(string name) : base(name)
        {
            Type = "Lipstick";
            Family = ProductFamily.LIPS;
        }
    }

    public class Blusher : Product
    {
        public Blusher(string name) : base(name)
        {
            Type = "Blusher";
            Family = ProductFamily.SKIN;
        }
    }

    public class Foundation : Product
    {
        public Foundation(string name) : base(name)
        {
            Type = "Foundation";
            Family = ProductFamily.SKIN;
        }
    }

    public enum ProductRange
    {
        QUEEN,
        BUDGET,
        PROFESSIONAL
    }

    public enum ProductFamily
    {
        EYES,
        LIPS,
        LASHES,
        SKIN
    }
}