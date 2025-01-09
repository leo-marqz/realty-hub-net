
using System;

namespace RealtyHub.Models
{
    public class Property : Entity
    {
        /// <summary>
        /// The title of the property
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the property
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The number of rooms in the property
        /// </summary>
        public int Rooms { get; set; }

        /// <summary>
        /// The number of parking spaces in the property
        /// </summary>
        public int Parking { get; set; }
        
        /// <summary>
        /// The number of bathrooms in the property
        /// </summary>
        public int WC { get; set; }

        /// <summary>
        /// Street name of the property
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// The city where the property is located - Latitude (map)
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The city where the property is located - Longitude (map)
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Property image
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The property has been published ?
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// The user who created the property
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The category of the property
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// The price of the property
        /// </summary>
        public Guid PriceId { get; set; }
    }
}