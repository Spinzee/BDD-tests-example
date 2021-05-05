namespace Products.Tests.Services
{
    using System;
    using Model.Common;
    using NUnit.Framework;
    using Service.QASBT;
    using Should;

    [TestFixture]
    public class QASBTMatchServiceTests
    {
        [TestCase("", "", "", "", "")]
        [TestCase("Blah", "", "", "", "")]
        [TestCase("", "Blah Road", "", "", "")]
        [TestCase("", "", "Blah Road", "", "")]
        [TestCase("Blah", "", "Road", "", "")]
        [TestCase("", "", "", "Blah Road", "")]
        [TestCase("Blah", "", "", "Road", "")]
        [TestCase("", "Blah", "", "Road", "")]
        [TestCase("", "", "Blah", "Road", "")]
        [TestCase("Blah", "", "Blah", "Road", "")]
        [TestCase("", "", "", "", "Blah Road")]
        [TestCase("Blah", "", "", "", "Road")]
        [TestCase("", "Blah", "", "", "Road")]
        [TestCase("Blah", "", "Blah", "", "Road")]
        [TestCase("", "", "", "Blah", "Road")]
        [TestCase("", "Blah", "", "Blah", "Road")]
        [TestCase("Blah", "", "Blah", "Blah", "Road")]
        [TestCase("Blah", "", "", "Blah", "Road")]
        [TestCase("Blah", "Blah", "", "Blah", "Road")]
        public void UnsupportedBitMaskValuesReturnZeroMatchesValues(string thoroughfareNumber, string thoroughfareName, string premise, string subPremises, string locality)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = "a, b, c, d" };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = "My Town",
                PremiseName = premise,
                SubPremises = subPremises,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(0);
            matchResults.Item2.ShouldEqual(0);
        }

        [TestCase("26 Grassmere Way, WATERLOOVILLE, Hampshire", "20", "GRASSMERE WAY", "WATERLOOVILLE", 4, 7)]
        [TestCase("26 Grassmere Way, WATERLOOVILLE, Hampshire", "22", "GRASSMERE WAY", "WATERLOOVILLE", 4, 7)]
        [TestCase("26 Grassmere Way, WATERLOOVILLE, Hampshire", "24", "GRASSMERE WAY", "WATERLOOVILLE", 4, 7)]
        [TestCase("26 Grassmere Way, WATERLOOVILLE, Hampshire", "26", "GRASSMERE WAY", "WATERLOOVILLE", 7, 7)]
        [TestCase("26 Grassmere Way, WATERLOOVILLE, Hampshire ", "26", "GRASSMERE WAY", "WATERLOOVILLE", 7, 7)]
        [TestCase("26 Grassmere Way WATERLOOVILLE Hampshire", "26", "GRASSMERE WAY", "WATERLOOVILLE", 7, 7)]
        [TestCase("26 Grassmere Way WATERLOOVILLE Hampshire ", "26", "GRASSMERE WAY", "WATERLOOVILLE", 7, 7)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Oak Tree House, Moreton Lane, THAME, Oxfordshire", "MORETON LANE", "THAME", "BATES LEYS FARM", 4, 6)]
        [TestCase("Oak Tree House, Moreton Lane, THAME, Oxfordshire", "MORETON LANE", "THAME", "FIELD VIEW", 4, 6)]
        [TestCase("Oak Tree House, Moreton Lane, THAME, Oxfordshire", "MORETON LANE", "THAME", "OAK TREE HOUSE", 6, 6)]
        [TestCase("Oak Tree House, Moreton Lane, THAME, Oxfordshire ", "MORETON LANE", "THAME", "OAK TREE HOUSE", 6, 6)]
        [TestCase("Oak Tree House, Moreton Lane THAME Oxfordshire", "MORETON LANE", "THAME", "OAK TREE HOUSE", 6, 6)]
        [TestCase("Oak Tree House, Moreton Lane THAME Oxfordshire ", "MORETON LANE", "THAME", "OAK TREE HOUSE", 6, 6)]
        public void BTAddressWithThoroughfareNamePostTownPremiseNameReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareName, string postTown, string premiseName, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Bristol City Council, Westend M S C P, Berkeley Place, BRISTOL", "", "BERKELEY PLACE", "BRISTOL", "THE TRIANGLE", 4, 6)]
        [TestCase("Bristol City Council, Westend M S C P, Berkeley Place, BRISTOL", "", "BERKELEY PLACE", "BRISTOL", "THE JACOBS BUILDING", 4, 6)]
        [TestCase("Bristol City Council, Westend M S C P, Berkeley Place, BRISTOL", "", "BERKELEY PLACE", "BRISTOL", "PROJECT OFFICE FURNITURE SALES", 4, 6)]
        [TestCase("Bristol City Council, Westend M S C P, Berkeley Place, BRISTOL", "P", "BERKELEY PLACE", "BRISTOL", "WESTEND M S C", 8, 8)]
        [TestCase("Bristol City Council, Westend M S C P, Berkeley Place, BRISTOL ", "P", "BERKELEY PLACE", "BRISTOL", "WESTEND M S C", 8, 8)]
        [TestCase("Bristol City Council Westend M S C P Berkeley Place BRISTOL ", "P", "BERKELEY PLACE", "BRISTOL", "WESTEND M S C", 8, 8)]
        [TestCase("Bristol City Council Westend M S C P Berkeley Place BRISTOL", "P", "BERKELEY PLACE", "BRISTOL", "WESTEND M S C", 8, 8)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownPremiseNameReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string premiseName, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Flat 6, 36 Waterloo Road, HAVANT, Hampshire", "36", "WATERLOO ROAD", "HAVANT", "FLAT 2", 6, 9)]
        [TestCase("Flat 6, 36 Waterloo Road, HAVANT, Hampshire", "36", "WATERLOO ROAD", "HAVANT", "FLAT 3", 6, 9)]
        [TestCase("Flat 6, 36 Waterloo Road, HAVANT, Hampshire", "36", "WATERLOO ROAD", "HAVANT", "FLAT 6", 9, 9)]
        [TestCase("Flat 6, 36 Waterloo Road, HAVANT, Hampshire ", "36", "WATERLOO ROAD", "HAVANT", "FLAT 6", 9, 9)]
        [TestCase("Flat 6 36 Waterloo Road HAVANT Hampshire", "36", "WATERLOO ROAD", "HAVANT", "FLAT 6", 9, 9)]
        [TestCase("Flat 6 36 Waterloo Road HAVANT Hampshire ", "36", "WATERLOO ROAD", "HAVANT", "FLAT 6", 9, 9)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownSubPremisesReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string subPremises, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                SubPremises = subPremises
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("10 Old Poor House Cottages, Mount Pleasant, ARUNDEL, West Sussex", "MOUNT PLEASANT", "ARUNDEL", "OLD POOR HOUSE COTTAGES", "1", 7, 9)]
        [TestCase("10 Old Poor House Cottages, Mount Pleasant, ARUNDEL, West Sussex", "MOUNT PLEASANT", "ARUNDEL", "FLINT COTTAGES", "2", 4, 9)]
        [TestCase("10 Old Poor House Cottages, Mount Pleasant, ARUNDEL, West Sussex", "MOUNT PLEASANT", "ARUNDEL", "OLD POOR HOUSE COTTAGES", "10", 9, 9)]
        [TestCase("10 Old Poor House Cottages, Mount Pleasant, ARUNDEL, West Sussex ", "MOUNT PLEASANT", "ARUNDEL", "OLD POOR HOUSE COTTAGES", "10", 9, 9)]
        [TestCase("10 Old Poor House Cottages Mount Pleasant ARUNDEL West Sussex", "MOUNT PLEASANT", "ARUNDEL", "OLD POOR HOUSE COTTAGES", "10", 9, 9)]
        [TestCase("10 Old Poor House Cottages Mount Pleasant ARUNDEL West Sussex ", "MOUNT PLEASANT", "ARUNDEL", "OLD POOR HOUSE COTTAGES", "10", 9, 9)]
        public void BTAddressWithThoroughfareNamePostTownPremiseSubPremisesReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareName, string postTown, string premiseName, string subPremises, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareName = thoroughfareName,
                PremiseName = premiseName,
                PostTown = postTown,
                SubPremises = subPremises
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Flat 18, Caradon Court, 1a Ellesmere Road, TWICKENHAM", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 7", 8, 11)]
        [TestCase("Flat 18, Caradon Court, 1a Ellesmere Road, TWICKENHAM", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 10", 8, 11)]
        [TestCase("Flat 18, Caradon Court, 1a Ellesmere Road, TWICKENHAM", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 18", 11, 11)]
        [TestCase("Flat 18, Caradon Court, 1a Ellesmere Road, TWICKENHAM ", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 18", 11, 11)]
        [TestCase("Flat 18 Caradon Court 1a Ellesmere Road TWICKENHAM", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 18", 11, 11)]
        [TestCase("Flat 18 Caradon Court 1a Ellesmere Road TWICKENHAM ", "1A", "ELLESMERE ROAD", "TWICKENHAM", "CARADON COURT", "FLAT 18", 11, 11)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownPremiseSubPremisesReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string premiseName, string subPremises, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName,
                SubPremises = subPremises
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("4 Canalside, Old Warwick Road, Lapworth, SOLIHULL, West Midlands", "1", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 6, 9)]
        [TestCase("4 Canalside, Old Warwick Road, Lapworth, SOLIHULL, West Midlands", "3", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 6, 9)]
        [TestCase("4 Canalside, Old Warwick Road, Lapworth, SOLIHULL, West Midlands", "4", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 9, 9)]
        [TestCase("4 Canalside, Old Warwick Road, Lapworth, SOLIHULL, West Midlands ", "4", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 9, 9)]
        [TestCase("4 Canalside Old Warwick Road Lapworth SOLIHULL West Midlands", "4", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 9, 9)]
        [TestCase("4 Canalside Old Warwick Road Lapworth SOLIHULL West Midlands ", "4", "OLD WARWICK ROAD", "SOLIHULL", "LAPWORTH", 9, 9)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownLocalityReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Wayside Cottage, Newnham, HENLEY-IN-ARDEN, West Midlands", "HENLEY-IN-ARDEN", "APPLETREES", "NEWNHAM", 4, 6)]
        [TestCase("Wayside Cottage, Newnham, HENLEY-IN-ARDEN, West Midlands", "HENLEY-IN-ARDEN", "ARDENCROFT", "NEWNHAM", 4, 6)]
        [TestCase("Wayside Cottage, Newnham, HENLEY-IN-ARDEN, West Midlands", "HENLEY-IN-ARDEN", "WAYSIDE COTTAGE", "NEWNHAM", 6, 6)]
        [TestCase("Wayside Cottage, Newnham, HENLEY-IN-ARDEN, West Midlands ", "HENLEY-IN-ARDEN", "WAYSIDE COTTAGE", "NEWNHAM", 6, 6)]
        [TestCase("Wayside Cottage Newnham HENLEY-IN-ARDEN West Midlands", "HENLEY-IN-ARDEN", "WAYSIDE COTTAGE", "NEWNHAM", 6, 6)]
        [TestCase("Wayside Cottage Newnham HENLEY-IN-ARDEN West Midlands ", "HENLEY-IN-ARDEN", "WAYSIDE COTTAGE", "NEWNHAM", 6, 6)]
        public void BTAddressWithPostTownPremiseNameLocalityReturnsCorrectMatchingValues(string picklistEntry, string postTown, string premiseName, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                PostTown = postTown,
                PremiseName = premiseName,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("The Cottage, Coton Road, Nether Whitacre, Coleshill, BIRMINGHAM", "COTON ROAD", "BIRMINGHAM", "THE CEDARS", "COLESHILL", 6, 8)]
        [TestCase("The Cottage, Coton Road, Nether Whitacre, Coleshill, BIRMINGHAM", "COTON ROAD", "BIRMINGHAM", "ROSE COTTAGE", "COLESHILL", 6, 8)]
        [TestCase("The Cottage, Coton Road, Nether Whitacre, Coleshill, BIRMINGHAM", "COTON ROAD", "BIRMINGHAM", "THE COTTAGE", "COLESHILL", 8, 8)]
        [TestCase("The Cottage, Coton Road, Nether Whitacre, Coleshill, BIRMINGHAM ", "COTON ROAD", "BIRMINGHAM", "THE COTTAGE", "COLESHILL", 8, 8)]
        [TestCase("The Cottage Coton Road Nether Whitacre Coleshill BIRMINGHAM", "COTON ROAD", "BIRMINGHAM", "THE COTTAGE", "COLESHILL", 8, 8)]
        [TestCase("The Cottage Coton Road Nether Whitacre Coleshill BIRMINGHAM ", "COTON ROAD", "BIRMINGHAM", "THE COTTAGE", "COLESHILL", 8, 8)]
        public void BTAddressWithThoroughfareNamePostTownPremiseNameLocalityReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareName, string postTown, string premiseName, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("Status Windows, Nawaris House, 857 Coronation Road, LONDON", "858", "CORONATION ROAD", "LONDON", "NAWARIS HOUSE", "WILLESDEN", 6, 9)]
        [TestCase("Status Windows, Nawaris House, 857 Coronation Road, LONDON", "857", "CORONATION ROAD", "LONDON", "NAWARIS HSE", "WILLESDEN", 7, 9)]
        [TestCase("Status Windows, Nawaris House, 857 Coronation Road, LONDON", "857", "CORONATION ROAD", "LONDON", "NAWARIS HOUSE", "WILLESDEN", 9, 9)]
        [TestCase("Status Windows, Nawaris House, 857 Coronation Road, LONDON ", "857", "CORONATION ROAD", "LONDON", "NAWARIS HOUSE", "WILLESDEN", 9, 9)]
        [TestCase("Status Windows Nawaris House 857 Coronation Road LONDON", "857", "CORONATION ROAD", "LONDON", "NAWARIS HOUSE", "WILLESDEN", 9, 9)]
        [TestCase("Status Windows Nawaris House 857 Coronation Road LONDON ", "857", "CORONATION ROAD", "LONDON", "NAWARIS HOUSE", "WILLESDEN", 9, 9)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownPremiseNameLocalityReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string premiseName, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("2 Welch Cottages, Lamyatt, SHEPTON MALLET, Somerset", "SHEPTON MALLET", "BATCH HOUSE", "2", "LAMYATT", 7, 9)]
        [TestCase("2 Welch Cottages, Lamyatt, SHEPTON MALLET, Somerset", "SHEPTON MALLET", "WELCH COTTAGES", "1", "LAMYATT", 6, 9)]
        [TestCase("2 Welch Cottages, Lamyatt, SHEPTON MALLET, Somerset", "SHEPTON MALLET", "WELCH COTTAGES", "2", "LAMYATT", 9, 9)]
        [TestCase("2 Welch Cottages, Lamyatt, SHEPTON MALLET, Somerset ", "SHEPTON MALLET", "WELCH COTTAGES", "2", "LAMYATT", 9, 9)]
        [TestCase("2 Welch Cottages Lamyatt SHEPTON MALLET Somerset", "SHEPTON MALLET", "WELCH COTTAGES", "2", "LAMYATT", 9, 9)]
        [TestCase("2 Welch Cottages Lamyatt SHEPTON MALLET Somerset ", "SHEPTON MALLET", "WELCH COTTAGES", "2", "LAMYATT", 9, 9)]
        public void BTAddressWithPostTownPremiseNameSubPremisesLocalityReturnsCorrectMatchingValues(string picklistEntry, string postTown, string premiseName, string subPremises, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                PostTown = postTown,
                PremiseName = premiseName,
                SubPremises = subPremises,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("2 Lower Farm Cottage, Bodymoor Heath Lane, Bodymoor Heath, SUTTON COLDFIELD, West Midlands", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "1", "BODYMOOR HEATH", 8, 11)]
        [TestCase("2 Lower Farm Cottage, Bodymoor Heath Lane, Bodymoor Heath, SUTTON COLDFIELD, West Midlands", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "1A", "BODYMOOR HEATH", 8, 11)]
        [TestCase("2 Lower Farm Cottage, Bodymoor Heath Lane, Bodymoor Heath, SUTTON COLDFIELD, West Midlands", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "2", "BODYMOOR HEATH", 11, 11)]
        [TestCase("2 Lower Farm Cottage, Bodymoor Heath Lane, Bodymoor Heath, SUTTON COLDFIELD, West Midlands ", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "2", "BODYMOOR HEATH", 11, 11)]
        [TestCase("2 Lower Farm Cottage Bodymoor Heath Lane Bodymoor Heath SUTTON COLDFIELD West Midlands", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "2", "BODYMOOR HEATH", 11, 11)]
        [TestCase("2 Lower Farm Cottage Bodymoor Heath Lane Bodymoor Heath SUTTON COLDFIELD West Midlands ", "BODYMOOR HEATH LANE", "SUTTON COLDFIELD", "LOWER FARM COTTAGE", "2", "BODYMOOR HEATH", 11, 11)]
        public void BTAddressWithThoroughfareNamePostTownPremiseNameSubPremisesLocalityReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareName, string postTown, string premiseName, string subPremises, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName,
                SubPremises = subPremises,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }

        [TestCase("12 Four Square Court, 405 Nelson Road, Whitton, HOUNSLOW", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "1", "WHITTON", 11, 13)]
        [TestCase("12 Four Square Court, 405 Nelson Road, Whitton, HOUNSLOW", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "7", "WHITTON", 10, 13)]
        [TestCase("12 Four Square Court, 405 Nelson Road, Whitton, HOUNSLOW", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "12", "WHITTON", 13, 13)]
        [TestCase("12 Four Square Court, 405 Nelson Road, Whitton, HOUNSLOW ", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "12", "WHITTON", 13, 13)]
        [TestCase("12 Four Square Court 405 Nelson Road Whitton HOUNSLOW", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "12", "WHITTON", 13, 13)]
        [TestCase("12 Four Square Court 405 Nelson Road Whitton HOUNSLOW ", "405", "NELSON ROAD", "HOUNSLOW", "FOUR SQUARE COURT", "12", "WHITTON", 13, 13)]
        public void BTAddressWithThoroughfareNumberThoroughfareNamePostTownPremiseNameSubPremisesLocalityReturnsCorrectMatchingValues(string picklistEntry, string thoroughfareNumber, string thoroughfareName, string postTown, string premiseName, string subPremises, string locality, int expectedMatches, int expectedTotalMatchTests)
        {
            // Arrange
            var quickAddress = new QasAddress { PicklistEntry = picklistEntry };
            var openReachAddress = new BTAddress
            {
                ThoroughfareNumber = thoroughfareNumber,
                ThoroughfareName = thoroughfareName,
                PostTown = postTown,
                PremiseName = premiseName,
                SubPremises = subPremises,
                Locality = locality
            };

            // Act 
            Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, quickAddress);

            // Assert
            matchResults.Item1.ShouldEqual(expectedMatches);
            matchResults.Item2.ShouldEqual(expectedTotalMatchTests);
        }
    }
}