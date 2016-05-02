﻿using System;
using System.Text;

namespace DDay.iCal
{
    public interface IAttachment : IEncodableDataType
    {
        /// <summary>
        /// The URI where the attachment information can be located.
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// A binary representation of the data that was loaded.
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// A unicode-encoded version of the data that was loaded.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// To specify the content type of a referenced object.
        /// This optional value should be an IANA-registered
        /// MIME type, if specified.
        /// </summary>
        string FormatType { get; set; }

        /// <summary>
        /// Gets/sets the encoding used to store the value.
        /// </summary>
        Encoding ValueEncoding { get; set; }

        /// <summary>
        /// Loads (fills) the <c>Data</c> property with the file designated at the given URI.
        /// </summary>
        void LoadDataFromUri();

        /// <summary>
        /// Loads (fills) the <c>Data</c> property with the file designated at the given URI.
        /// </summary>
        /// <param name="username">The username to supply for credentials</param>
        /// <param name="password">The pasword to supply for credentials</param>
        void LoadDataFromUri(string username, string password);

        /// <summary>
        /// Loads (fills) the <c>Data</c> property with the file designated at the given URI.
        /// </summary>
        /// <param name="uri">The Uri from which to download the <c>Data</c></param>
        /// <param name="username">The username to supply for credentials</param>
        /// <param name="password">The pasword to supply for credentials</param>
        void LoadDataFromUri(Uri uri, string username, string password);
    }
}