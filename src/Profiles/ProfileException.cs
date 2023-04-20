namespace Mercurius.Profiles {
     [System.Serializable]
    public class ProfileException : System.Exception
    {
        public ProfileException() { }
        public ProfileException(string message) : base(message) { }
        public ProfileException(string message, System.Exception inner) : base(message, inner) { }
        protected ProfileException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}