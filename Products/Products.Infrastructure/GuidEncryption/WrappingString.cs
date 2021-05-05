using System.Text;

namespace Products.Infrastructure.GuidEncryption
{
    public class WrappingString
    {
        private readonly StringBuilder _builder;

        public int Position { get; private set; }

        public char CurrentValue
        {
            get
            {
                return _builder[Position];
            }

            set
            {
                _builder[Position] = value;
            }
        }

        public WrappingString(string input) : this(input, 0)
        {

        }

        public WrappingString(string input, int startPosition)
        {
            _builder = new StringBuilder(input);
            Position = StartingPointLoop(startPosition);
        }

        public WrappingString(int length, int startPosition)
        {
            _builder = new StringBuilder(length);
            _builder.Append(new string(' ', length));
            Position = StartingPointLoop(startPosition);
        }

        public WrappingString(int length)
        {
            _builder = new StringBuilder(length);
            _builder.Append(new string(' ', length));
            Position = 0;
        }

        private int StartingPointLoop(int startingPoint)
        {
            int point = startingPoint % _builder.Length;
            return point;
        }

        public void Insert(int insertIndex, char valueToInsert)
        {
            _builder.Insert(insertIndex, valueToInsert);
        }

        public void Replace(char oldChar, char newChar)
        {
            _builder.Replace(oldChar, newChar);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public void MoveNext()
        {
            MoveForward(1);
        }

        public void Append(string characterToAdd)
        {
            _builder.Append(characterToAdd);
        }

        public void MoveForward(int steps)
        {
            // If the number of steps causes us to move past the end of the array then resposition at the begining
            Position = (Position + steps) % _builder.Length;
        }

        public char PreviousCharacter()
        {
            int previousPosition = Position - 1;

            if (previousPosition < 0)
            {
                previousPosition = _builder.Length - 1;
            }

            return _builder[previousPosition];
        }

        public void Replace(char oldCharacter, char newCharacter, int indexOfCharacter, int numberToReplace)
        {
            _builder.Replace(oldCharacter, newCharacter, indexOfCharacter, numberToReplace);
        }
    }
}
