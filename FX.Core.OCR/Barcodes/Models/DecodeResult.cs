using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Models
{
    public class DecodeResult
    {
        private ZXing.Result originalResult;
        public string FileName { get; set; }
        public string BarcodeFormat { get; set; }
        public string Text { get; set; }
        public bool Success { get; set; } = true;
        public string Note { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public TimeSpan Duration { get; set; }
        public int PageInTiff { get; set; }

        public DecodeResult() { }
        public DecodeResult(ZXing.Result originalResult)
        {
            this.originalResult = originalResult;
            ParseLoadedResult();
        }

        protected void ParseLoadedResult()
        {
            if (originalResult == null)
            {
                Success = false;
                Note = "Barcode not found";
                return;
            }

            BarcodeFormat = originalResult.BarcodeFormat.ToString();
            Text = originalResult.Text;
        }

        public override string ToString()
        {
            return "Success: " + (Success ? "true" : "false") + "-->" +
                "Text: " + Text + "-->" +
                "Format: " + BarcodeFormat + "-->" +
                "Note: " + Note + "-->";
        }
    }
}
