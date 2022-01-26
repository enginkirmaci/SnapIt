using System;
using System.Text.RegularExpressions;
using System.Windows;
using ThinkSharp.Licensing;
using ThinkSharp.Licensing.Signing;

namespace SnapScreen.Test
{
    /// <summary>
    /// Interaction logic for License.xaml
    /// </summary>
    public partial class LicenseUI : Window
    {
        private string privateKey;
        private string publicKey;

        public LicenseUI()
        {
            InitializeComponent();

            SigningKeyPair pair = Lic.KeyGenerator.GenerateRsaKeyPair();

            privateKey = "BwIAAACkAABSU0EyAAQAAAEAAQCBY+E7Jzq11V5MZ0uq7ti3iFb/6X/z9R6G4oaPIZyUQN1Oxz650QICctnnRjOsME2uCicInqqGEC+X/HJhH8e/YZtRdqt1Bb2rNnP/TDhLdUEOb6V9fOUWsGqjQ8u6BTaIX/0bSUZp26IHkJMpynQdyPEta/b0D7XnA/ORFJzdnY9dLstM9XkppPNE5E43P6VGc9p80y4Blg+jV0i6MgtkyiNYzPoJG98bOC0zQWi4BMLPGHnNEGBRSwxrVggSAMfvxVfO8KPd2zLOqXfsMMdRRyj/ppdOR0qph5u5+QkN+LIXMjxyYoZQCi2RmAAdBWHVz7ECP5wv0nnXkD+cSxXL3WRzHFJEX2RDDSj2a3rAW97iWmpKi3OLtk3boA5OkI5iqFrgsLdNt8WnH4C7g2sLA9V/vdFiTe4kS808dVQWHKmHZJ1faxpT9cslvRAvYM7Gr13pEZ1E97QGEvkObPugIaA1t+pv7tjl4kzrlofprRPMVRBnIglTF2JgTyNxqHOYxrMnZ9DxE0k8jE1PG60RI1qneypdsGCW9tyWSCy4ryuZXC+hnU3dg8bp3iMnVhrjmJ/PBPwsNL2ynjTkTP8W8R0CBZlPJy1Uo9imb382qITE3j2chaOUfQ8WjtmpAGmxmGtY+s33woKkvyWWUUHwKo+G9M/Ywa8HHTf0Tg0X8b7yB5hBF5udv4+NUroNP9Gq4INK3nGx6zmS2Kh1YGzyBw1Pb96bC4zWmiLPCUAPNq1fMMwZmFOL2671p/6BnQQ=";
            publicKey = "BgIAAACkAABSU0ExAAQAAAEAAQCBY+E7Jzq11V5MZ0uq7ti3iFb/6X/z9R6G4oaPIZyUQN1Oxz650QICctnnRjOsME2uCicInqqGEC+X/HJhH8e/YZtRdqt1Bb2rNnP/TDhLdUEOb6V9fOUWsGqjQ8u6BTaIX/0bSUZp26IHkJMpynQdyPEta/b0D7XnA/ORFJzdnQ==";

            this.Loaded += LicenseUI_Loaded;
        }

        private void LicenseUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void generateBtn_Click(object sender, RoutedEventArgs e)
        {
            SignedLicense license = Lic.Builder
                .WithRsaPrivateKey(privateKey)                                           // .WithSigner(ISigner)
                .WithoutHardwareIdentifier() // .WithoutHardwareIdentifier()
                .WithSerialNumber(SerialNumber.Create("SSQ")) //.WithoutSerialNumber()                 // .WithoutSerialNumber()
                .WithoutExpiration()                                             // .ExpiresIn(TimeSpan), .ExpiresOn(DateTime)
                .WithProperty("Name", "Bill Gates")
                .SignAndCreate();

            var str = license.Serialize();

            licenseText.Text = string.Join("\n", Regex.Split(str, @"(?<=\G.{50})", RegexOptions.Singleline));
        }

        private void validateBtn_Click(object sender, RoutedEventArgs e)
        {
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