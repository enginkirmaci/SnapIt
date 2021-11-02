using System;
using System.Text.RegularExpressions;
using System.Windows;
using ThinkSharp.Licensing;
using ThinkSharp.Licensing.Signing;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for License.xaml
    /// </summary>
    public partial class LicenseUI : Window
    {
        private string passPhrase = "SnapScreen";
        private string privateKey;
        private string publicKey;

        public LicenseUI()
        {
            InitializeComponent();

            //var keyGenerator = KeyGenerator.Create();
            //var keyPair = keyGenerator.GenerateKeyPair();
            //privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
            //publicKey = keyPair.ToPublicKeyString();

            SigningKeyPair pair = Lic.KeyGenerator.GenerateRsaKeyPair();
            privateKey = pair.PrivateKey;
            //publicKey = "BgIAAACkAABSU0ExAAQAAAEAAQBpQYIw1uU1OsZm6ittd9St9++OfxdgmkrMWGNVAoqdcFaHI2YNX2V7NEnxlnQqOZogbO8EKUf2dXhFpx6tbM9yH6EEIGfvrXezr8nNIZAAkQ4OuE3PUmoFXUTNT6U5JAVwwaIEL5aEHgHlmIY9x6z3z83Et4PpcipdxxFVOpTiuQ==";
            publicKey = pair.PublicKey;
        }

        private void generateBtn_Click(object sender, RoutedEventArgs e)
        {
            //var license = License.New()
            //   .WithUniqueIdentifier(Guid.NewGuid())
            //   .As(LicenseType.Standard)
            //   .LicensedTo("John Doe", "john.doe@example.com")
            //   .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            //licenseText.Text = license.ToString();

            SignedLicense license = Lic.Builder
                .WithRsaPrivateKey(privateKey)                                           // .WithSigner(ISigner)
                .WithoutHardwareIdentifier() // .WithoutHardwareIdentifier()
                .WithoutSerialNumber()                 // .WithoutSerialNumber()
                .WithoutExpiration()                                             // .ExpiresIn(TimeSpan), .ExpiresOn(DateTime)
                .WithProperty("Name", "Bill Gates")
                .SignAndCreate();

            //licenseText.Text = license.Serialize();

            var str = license.Serialize();

            licenseText.Text = string.Join("\n", Regex.Split(str, @"(?<=\G.{50})", RegexOptions.Singleline));
        }

        private void validateBtn_Click(object sender, RoutedEventArgs e)
        {
            //resultText.Text = string.Empty;
            //var license = License.Load(validateText.Text);
            //var validationFailures = license.Validate()
            //                    .Signature(publicKey)
            //                    .When(lic => lic.Type == LicenseType.Standard)
            //                    .AssertValidLicense();

            //foreach (var failure in validationFailures)
            //    resultText.Text += "\n" + failure.GetType().Name + ": " + failure.Message + " - " + failure.HowToResolve;
            try
            {
                SignedLicense license = Lic.Verifier
                    .WithRsaPublicKey(publicKey)       // .WithSigner(ISigner)
                    .WithApplicationCode("SSQ")         // .WithoutApplicationCode
                    .LoadAndVerify(validateText.Text.Replace("\n", string.Empty));

                resultText.Text = license.SerializeAsPlainText();
            }
            catch (Exception ex)
            {
                resultText.Text = ex.ToString();
            }
        }
    }
}