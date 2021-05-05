using System;
using System.Linq;

namespace Products.Infrastructure.GuidEncryption
{
    public class SpinningNumber
    {
        private readonly int _baseNumber;

        private readonly char[] _baseNumberArray =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E',
            'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        private int _currentValue;

        public SpinningNumber(int baseNumber)
        {
            if (baseNumber > _baseNumberArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(baseNumber), baseNumber, "Base Number Is To Large");
            }

            _baseNumber = baseNumber;
            _currentValue = 0;
        }

        public int BaseNumber => _baseNumber;

        public void SetCurrentValue(char valueGiven)
        {
            int charactersNumericValue = CharToNumericValue(char.ToUpper(valueGiven));

            if (charactersNumericValue == -1 || !IsValueValid(charactersNumericValue))
            {
                throw new ArgumentException(
                    "Unable to validate string, one or more characters are not supported",
                    nameof(valueGiven));
            }

            _currentValue = charactersNumericValue;
        }

        public int CharToNumericValue(char valueGiven)
        {
            valueGiven = char.ToUpper(valueGiven);

            if (!IsCharacterValueInRange(valueGiven))
            {
                throw new ArgumentOutOfRangeException(nameof(valueGiven), valueGiven, "Character given was not beetween 0-9, or A-Z");
            }

            int charactersNumericValue = -1;

            for (int i = 0; i < _baseNumberArray.Length && charactersNumericValue == -1; i++)
            {
                if (_baseNumberArray[i] == valueGiven)
                {
                    charactersNumericValue = i;
                }
            }

            return charactersNumericValue;
        }

        public char GetCurrentValue()
        {
            return _baseNumberArray[_currentValue];
        }

        public bool IsValueValid(char valueGiven)
        {
            int numericValue = CharToNumericValue(valueGiven);
            return IsValueValid(numericValue);
        }

        public bool IsValueValid(string valueGiven)
        {
            bool isValid = true;

            for (int i = 0; i < valueGiven.Length && isValid; i++)
            {
                isValid = IsValueValid(valueGiven[i]);
            }

            return isValid;
        }

        public char Spin(int step, Direction direction)
        {
            step = step % _baseNumber;
            return SpinIt(step, direction);
        }


        public char Next()
        {
            return Spin(1, Direction.Forward);
        }


        public char Previous()
        {
            return Spin(1, Direction.Back);
        }


        public char ConvertToChar(int valueGiven)
        {
            return _baseNumberArray[valueGiven % _baseNumber];
        }


        public char Spin(char step, Direction direction)
        {
            step = char.ToUpper(step);

            if (!IsValueValid(step))
            {
                throw new ArgumentException("Not a valid Value to spin", nameof(step));
            }

            int theStep = CharToNumericValue(step) % _baseNumber;

            return SpinIt(theStep, direction);
        }

        private char SpinIt(int theStep, Direction direction)
        {
            if (direction == Direction.Forward)
            {
                _currentValue = (_currentValue + theStep) % _baseNumber;
            }
            else
            {
                _currentValue = (_currentValue - theStep) % _baseNumber;

                if (_currentValue < 0)
                {
                    _currentValue = _currentValue + _baseNumber;
                }
            }

            return _baseNumberArray[_currentValue];
        }

        private bool IsCharacterValueInRange(char valueGiven)
        {
            return _baseNumberArray.Any(t => t == valueGiven);
        }

        private bool IsValueValid(int valueGiven)
        {
            return valueGiven >= 0 && valueGiven < _baseNumber;
        }
    }
}