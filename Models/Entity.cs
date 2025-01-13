
using System;

namespace RealtyHub.Models
{
    public abstract class Entity
    {
        /// <summary>
        /// The unique identifier of the entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates if the entity is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// The date and time the entity was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time the entity was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The date and time the entity was deleted (UTC).
        /// Null if the entity has not been deleted.
        /// </summary>
        public DateTime DeletedAt { get; set; }
    }
}