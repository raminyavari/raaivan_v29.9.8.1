using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Collections.Specialized;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class Captcha
    {
        private static string _SessionVariableName = "CaptchaString";

        public static bool check(HttpContext context, string input)
        {
            if (string.IsNullOrEmpty(input)) input = string.Empty;

            string sessionValue = null;

            try { sessionValue = context.Session[_SessionVariableName].ToString(); }
            catch { }

            if (RaaiVanSettings.SAASBasedMultiTenancy)
            {
                return (!string.IsNullOrEmpty(sessionValue) && sessionValue == input.ToLower()) ||
                    !string.IsNullOrEmpty(input) && input.Length > 100; //google validation API doesn't work with Iran's IP

                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(RaaiVanSettings.Google.Captcha.SecretKey) || 
                    string.IsNullOrEmpty(RaaiVanSettings.Google.Captcha.ValidationURL)) return false;

                NameValueCollection data = new NameValueCollection();
                data.Add("secret", RaaiVanSettings.Google.Captcha.SecretKey);
                data.Add("response", input);

                Dictionary<string, object> res = 
                    PublicMethods.fromJSON(PublicMethods.web_request(RaaiVanSettings.Google.Captcha.ValidationURL, data));

                return PublicMethods.get_dic_value<bool>(res, "success", defaultValue: false);
            }
            else
                return !string.IsNullOrEmpty(sessionValue) && sessionValue == input.ToLower();
        }

        public static void generate(HttpContext context, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            
            Graphics gfxCaptchaImage = Graphics.FromImage(bitmap);
            gfxCaptchaImage.PageUnit = GraphicsUnit.Pixel;
            gfxCaptchaImage.SmoothingMode = SmoothingMode.HighQuality;
            gfxCaptchaImage.Clear(Color.White);

            int salt = PublicMethods.get_random_number(6);

            context.Session[_SessionVariableName] = salt.ToString().ToLower();

            string randomString = (salt).NumberToText(Language.Persian);

            StringFormat format = new StringFormat();
            int faLCID = new System.Globalization.CultureInfo("fa-IR").LCID;
            format.SetDigitSubstitution(faLCID, StringDigitSubstitute.National);
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            format.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            Font font = new Font("Tahoma", 10);

            GraphicsPath path = new GraphicsPath();

            path.AddString(randomString, font.FontFamily, (int)font.Style, (gfxCaptchaImage.DpiY * font.SizeInPoints / 72),
                new Rectangle(0, 0, width, height), format);

            gfxCaptchaImage.DrawPath(Pens.Navy, path);

            int distortion = new Random().Next(-10, 10);

            using (Bitmap copy = (Bitmap)bitmap.Clone())
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int newX = (int)(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                        int newY = (int)(y + (distortion * Math.Cos(Math.PI * x / 64.0)));
                        if (newX < 0 || newX >= width) newX = 0;
                        if (newY < 0 || newY >= height) newY = 0;
                        bitmap.SetPixel(x, y, copy.GetPixel(newX, newY));
                    }
                }
            }

            gfxCaptchaImage.DrawImage(bitmap, new Point(0, 0));
            gfxCaptchaImage.Flush();

            context.Response.ContentType = "image/jpeg";
            context.Response.DisableKernelCache();
            bitmap.Save(context.Response.OutputStream, ImageFormat.Jpeg);

            font.Dispose();
            gfxCaptchaImage.Dispose();
            bitmap.Dispose();
        }
    }

    /// <summary>
    /// Number to word languages
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// English Language
        /// </summary>
        English,

        /// <summary>
        /// Persian Language
        /// </summary>
        Persian
    }

    /// <summary>
    /// Digit's groups
    /// </summary>
    public enum DigitGroup
    {
        /// <summary>
        /// Ones group
        /// </summary>
        Ones,

        /// <summary>
        /// Teens group
        /// </summary>
        Teens,

        /// <summary>
        /// Tens group
        /// </summary>
        Tens,

        /// <summary>
        /// Hundreds group
        /// </summary>
        Hundreds,

        /// <summary>
        /// Thousands group
        /// </summary>
        Thousands
    }

    /// <summary>
    /// Equivalent names of a group 
    /// </summary>
    public class NumberWord
    {
        /// <summary>
        /// Digit's group
        /// </summary>
        public DigitGroup Group { set; get; }

        /// <summary>
        /// Number to word language
        /// </summary>
        public Language Language { set; get; }

        /// <summary>
        /// Equivalent names
        /// </summary>
        public IList<string> Names { set; get; }
    }

    /// <summary>
    /// Convert a number into words
    /// </summary>
    public static class HumanReadableInteger
    {
        #region Fields (4)

        private static readonly IDictionary<Language, string> And = new Dictionary<Language, string>
		{
			{ Language.English, " " },
			{ Language.Persian, " و " } 
		};
        private static readonly IList<NumberWord> NumberWords = new List<NumberWord>
		{
			new NumberWord { Group= DigitGroup.Ones, Language= Language.English, Names=
				new List<string> { string.Empty, "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" }},
			new NumberWord { Group= DigitGroup.Ones, Language= Language.Persian, Names=
				new List<string> { string.Empty, "يك", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" }},

			new NumberWord { Group= DigitGroup.Teens, Language= Language.English, Names=
				new List<string> { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" }},
			new NumberWord { Group= DigitGroup.Teens, Language= Language.Persian, Names=
				new List<string> { "ده", "يازده", "دوازده", "سيزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" }},

			new NumberWord { Group= DigitGroup.Tens, Language= Language.English, Names=
				new List<string> { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" }},
			new NumberWord { Group= DigitGroup.Tens, Language= Language.Persian, Names=
				new List<string> { "بيست", "سي", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" }},

			new NumberWord { Group= DigitGroup.Hundreds, Language= Language.English, Names=
				new List<string> {string.Empty, "One Hundred", "Two Hundred", "Three Hundred", "Four Hundred", 
					"Five Hundred", "Six Hundred", "Seven Hundred", "Eight Hundred", "Nine Hundred" }},
			new NumberWord { Group= DigitGroup.Hundreds, Language= Language.Persian, Names=
				new List<string> {string.Empty, "يكصد", "دويست", "سيصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد" , "نهصد" }},

			new NumberWord { Group= DigitGroup.Thousands, Language= Language.English, Names=
			  new List<string> { string.Empty, " Thousand", " Million", " Billion"," Trillion", " Quadrillion", " Quintillion", " Sextillian",
			" Septillion", " Octillion", " Nonillion", " Decillion", " Undecillion", " Duodecillion", " Tredecillion",
			" Quattuordecillion", " Quindecillion", " Sexdecillion", " Septendecillion", " Octodecillion", " Novemdecillion",
			" Vigintillion", " Unvigintillion", " Duovigintillion", " 10^72", " 10^75", " 10^78", " 10^81", " 10^84", " 10^87",
			" Vigintinonillion", " 10^93", " 10^96", " Duotrigintillion", " Trestrigintillion" }},
			new NumberWord { Group= DigitGroup.Thousands, Language= Language.Persian, Names=
			  new List<string> { string.Empty, " هزار", " ميليون", " ميليارد"," تريليون", " Quadrillion", " Quintillion", " Sextillian",
			" Septillion", " Octillion", " Nonillion", " Decillion", " Undecillion", " Duodecillion", " Tredecillion",
			" Quattuordecillion", " Quindecillion", " Sexdecillion", " Septendecillion", " Octodecillion", " Novemdecillion",
			" Vigintillion", " Unvigintillion", " Duovigintillion", " 10^72", " 10^75", " 10^78", " 10^81", " 10^84", " 10^87",
			" Vigintinonillion", " 10^93", " 10^96", " Duotrigintillion", " Trestrigintillion" }},
		};
        private static readonly IDictionary<Language, string> Negative = new Dictionary<Language, string>
		{
			{ Language.English, "Negative " },
			{ Language.Persian, "منهاي " } 
		};
        private static readonly IDictionary<Language, string> Zero = new Dictionary<Language, string>
		{
			{ Language.English, "Zero" },
			{ Language.Persian, "صفر" } 
		};

        #endregion Fields

        #region Methods (7)

        // Public Methods (5) 

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this int number, Language language)
        {
            return NumberToText((long)number, language);
        }


        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this uint number, Language language)
        {
            return NumberToText((long)number, language);
        }

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this byte number, Language language)
        {
            return NumberToText((long)number, language);
        }

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this decimal number, Language language)
        {
            return NumberToText((long)number, language);
        }

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this double number, Language language)
        {
            return NumberToText((long)number, language);
        }

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public static string NumberToText(this long number, Language language)
        {
            if (number == 0)
            {
                return Zero[language];
            }

            if (number < 0)
            {
                return Negative[language] + NumberToText(-number, language);
            }

            return wordify(number, language, string.Empty, 0);
        }
        // Private Methods (2) 

        private static string getName(int idx, Language language, DigitGroup group)
        {
            return NumberWords.Where(x => x.Group == group && x.Language == language).First().Names[idx];
        }

        private static string wordify(long number, Language language, string leftDigitsText, int thousands)
        {
            if (number == 0)
            {
                return leftDigitsText;
            }

            var wordValue = leftDigitsText;
            if (wordValue.Length > 0)
            {
                wordValue += And[language];
            }

            if (number < 10)
            {
                wordValue += getName((int)number, language, DigitGroup.Ones);
            }
            else if (number < 20)
            {
                wordValue += getName((int)(number - 10), language, DigitGroup.Teens);
            }
            else if (number < 100)
            {
                wordValue += wordify(number % 10, language, getName((int)(number / 10 - 2), language, DigitGroup.Tens), 0);
            }
            else if (number < 1000)
            {
                wordValue += wordify(number % 100, language, getName((int)(number / 100), language, DigitGroup.Hundreds), 0);
            }
            else
            {
                wordValue += wordify(number % 1000, language, wordify(number / 1000, language, string.Empty, thousands + 1), 0);
            }

            if (number % 1000 == 0) return wordValue;
            return wordValue + getName(thousands, language, DigitGroup.Thousands);
        }

        #endregion Methods
    }
}
