
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
        /// The date and time the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time the entity was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The date and time the entity was deleted
        /// </summary>
        public DateTime DeletedAt { get; set; }
    }
}