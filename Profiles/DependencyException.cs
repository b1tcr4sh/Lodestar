using System;

namespace Mercurius.Profiles {
    [System.Serializable]
    public class DependencyException : Exception {
        public DependencyException() { }
        public DependencyException(string message) : base(message) { }
        public DependencyException(string message, System.Exception inner) : base(message, inner) { }
        protected DependencyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}