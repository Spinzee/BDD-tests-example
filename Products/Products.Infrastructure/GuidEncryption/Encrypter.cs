using System;
using System.Globalization;

namespace Products.Infrastructure.GuidEncryption
{
    public class Encrypter
    {
        private const int MaxSpinNumber = 36;
        private const int Step1SpinIterationValue = 3;
        private const int Step1IterationValue = 6;

        private int CountCharacters(string messageToEncrypt)
        {
            return messageToEncrypt.Length;
        }

        public string Encrypt(string messageToEncrypt)
        {
            if (messageToEncrypt.Length <= 0)
            {
                throw new ArgumentNullException(messageToEncrypt, "Can't Encrypt String is empty");
            }

            int messageLength = CountCharacters(messageToEncrypt);

            int encryptionKey = GenerateKey(messageLength);
            int step1SpinValue = messageLength + Step1SpinIterationValue;

            int unencryptedStartPostion;
            int encryptedStartPosition;

            SpinningNumber spinNumber = new SpinningNumber(MaxSpinNumber);

            WrappingString unencryptedString = new WrappingString(messageToEncrypt, encryptionKey - 1);

            WrappingString encryptedString = new WrappingString(messageLength, encryptionKey + Step1IterationValue);

            unencryptedStartPostion = unencryptedString.Position;
            encryptedStartPosition = encryptedString.Position;

            spinNumber.SetCurrentValue(unencryptedString.CurrentValue);

            if (IsValueEven(encryptionKey))
            {
                spinNumber.Spin(step1SpinValue, Direction.Forward);
            }
            else
            {
                spinNumber.Spin(step1SpinValue, Direction.Back);
            }

            encryptedString.CurrentValue = spinNumber.GetCurrentValue();

            encryptedString.MoveNext();
            unencryptedString.MoveNext();

            do
            {
                spinNumber.SetCurrentValue(unencryptedString.CurrentValue);
                int characterIntValue = spinNumber.CharToNumericValue(unencryptedString.PreviousCharacter());

                if (IsValueEven(characterIntValue))
                {
                    spinNumber.Spin(characterIntValue, Direction.Forward);
                }
                else
                {
                    spinNumber.Spin(characterIntValue, Direction.Back);
                }

                encryptedString.CurrentValue = spinNumber.GetCurrentValue();

                unencryptedString.MoveNext();
                encryptedString.MoveNext();
            }
            while (unencryptedString.Position != unencryptedStartPostion
                   && encryptedString.Position != encryptedStartPosition);

            char charMessagekey = spinNumber.ConvertToChar(encryptionKey);
            encryptedString.Append(charMessagekey.ToString(CultureInfo.InvariantCulture));

            return encryptedString.ToString();
        }

        public string Decrypt(string encryptedMessage)
        {

            int unencryptedStartPostion;
            int encryptedStartPosition;

            if (encryptedMessage.Length <= 0)
            {
                throw new ArgumentNullException(encryptedMessage, "Can't Decrypt Empty String");
            }

            SpinningNumber spinNumber = new SpinningNumber(MaxSpinNumber);

            int unencryptedStringLength = encryptedMessage.Length - 1;

            int encryptionKey = spinNumber.CharToNumericValue(encryptedMessage[encryptedMessage.Length - 1]);

            encryptedMessage = encryptedMessage.Remove(encryptedMessage.Length - 1, 1);

            WrappingString encryptedString = new WrappingString(encryptedMessage, encryptionKey + Step1IterationValue);


            WrappingString unencryptedString = new WrappingString(unencryptedStringLength, encryptionKey - 1);


            encryptedStartPosition = encryptedString.Position;
            unencryptedStartPostion = unencryptedString.Position;

            spinNumber.SetCurrentValue(encryptedString.CurrentValue);

            if (IsValueEven(encryptionKey))
            {
                spinNumber.Spin(unencryptedStringLength + Step1SpinIterationValue, Direction.Back);
            }
            else
            {
                spinNumber.Spin(unencryptedStringLength + Step1SpinIterationValue, Direction.Forward);
            }

            unencryptedString.CurrentValue = spinNumber.GetCurrentValue();

            unencryptedString.MoveNext();
            encryptedString.MoveNext();

            do
            {
                char encryptedCharacter = encryptedString.CurrentValue;
                char previousUnencryptedCharacter = unencryptedString.PreviousCharacter();

                spinNumber.SetCurrentValue(encryptedCharacter);

                if (IsValueEven(spinNumber.CharToNumericValue(previousUnencryptedCharacter)))
                {
                    spinNumber.Spin(previousUnencryptedCharacter, Direction.Back);
                }
                else
                {
                    spinNumber.Spin(previousUnencryptedCharacter, Direction.Forward);
                }

                unencryptedString.CurrentValue = spinNumber.GetCurrentValue();

                encryptedString.MoveNext();
                unencryptedString.MoveNext();
            }
            while (unencryptedString.Position != unencryptedStartPostion
                   && encryptedString.Position != encryptedStartPosition);

            return unencryptedString.ToString();
        }

        private int GenerateKey(int messageLength)
        {
            ////makes sure the random number(key) is no larger than the max spinning number
            Random key = new Random();
            return key.Next(1, Math.Min(messageLength, MaxSpinNumber - 1));
        }

        private bool IsValueEven(int number)
        {
            return number % 2 == 0 && number != 1 && number != 0;
        }




    }
}
