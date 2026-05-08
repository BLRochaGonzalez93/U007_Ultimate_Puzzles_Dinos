// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("STE0EkEmKzbdJAEkwrJrWXg8GrHXDaAi877Jp8919C+sCISlGpTb5HqvElYit2bN6NIPiPd+8tkMRBvt0KMr36kxDiV5xN0XwwscpzMB7o0JBs62n4R6Rds+HKj6LZNPf6E3w94pfsVEfu/ErzsEwTQGwiRQT35Cy40rcmrurfvyIKB8vmSV/M7LtQF6yEtoekdMQ2DMAsy9R0tLS09KSalDutglJlFbJymcAchQHtc+S25cjWGjjwoTxljUp5yVS7A0cllOVAKLlk3/dyn6+y1IBYhdxgnC9zWJ9chLRUp6yEtASMhLS0rv4fSz87fpshUYGHFTUn1OOMBuqF1v4fxqq7OKimxskjiC0GLCOO1/g2px4wwb4PFYYV7JayP69UhJS0pL");
        private static int[] order = new int[] { 10,7,3,9,5,9,11,10,8,13,13,13,13,13,14 };
        private static int key = 74;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
