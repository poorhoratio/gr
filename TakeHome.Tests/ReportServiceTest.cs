using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TakeHome.Console.Models;
using TakeHome.Console.Queries;
using TakeHome.Console.Services;

namespace TakeHome.Tests
{
    public class ReportServiceTests
    {
        private Mock<IGetAccounts> _mockGetOldAccounts;
        private Mock<IGetAccounts> _mockGetNewAccounts;
        private ReportsService _sut;

        [SetUp]
        public void Setup()
        {
            _mockGetOldAccounts = new Mock<IGetAccounts>();
            _mockGetNewAccounts = new Mock<IGetAccounts>();
            _sut = new ReportsService(_mockGetOldAccounts.Object, _mockGetNewAccounts.Object);
        }

        [Test]
        public void GetChanges_AllRecordsMatch()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0"),
                    CreateAccount("1", "Name1", "Email1")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0"),
                    CreateAccount("1", "Name1", "Email1")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(0, changes.Count);
        }

        [Test]
        public void GetChanges_NewCsvCheck()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account>());

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("New,0,,,Name0,Email0", changes[0].Csv);
        }

        [Test]
        public void GetChanges_MissingCsvCheck()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account>());

            var changes = _sut.GetChanges();
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("Missing,0,Name0,Email0,,", changes[0].Csv);
        }

        [Test]
        public void GetChanges_CorruptCsvCheck()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "OldName0", "OldEmail0")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "NewName0", "NewEmail0")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("Corrupted,0,OldName0,OldEmail0,NewName0,NewEmail0", changes[0].Csv);
        }

        [Test]
        public void GetChanges_AllRecordsAreNew()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account>());

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0"),
                    CreateAccount("1", "Name1", "Email1")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(2, changes.Count(c => c.ChangeType == ChangeType.New));
        }

        [Test]
        public void GetChanges_AllRecordsAreOld()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "Name0", "Email0"),
                    CreateAccount("1", "Name1", "Email1")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account>());

            var changes = _sut.GetChanges();
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(2, changes.Count(c => c.ChangeType == ChangeType.Missing));
        }

        [Test]
        public void GetChanges_AllRecordsAreCorrupted()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "OldName0", "OldEmail0"),
                    CreateAccount("1", "OldName1", "OldEmail1")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "CorruptName0", "Email0"),
                    CreateAccount("1", "CorruptName1", "Email1")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(2, changes.Count(c => c.ChangeType == ChangeType.Corrupted));
        }

        [Test]
        public void GetChanges_OneMissingOneNewOneCorrupted()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "MissingName", "MissingEmail"),
                    CreateAccount("1", "OldName1", "OldEmail1")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("1", "CorruptName1", "Email1"),
                    CreateAccount("2", "NewName", "NewEmail")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.Missing));
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.New));
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.Corrupted));
        }

        [Test]
        public void GetChanges_OneMissingOneNewOneCorruptedOneMatch()
        {
            _mockGetOldAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("0", "MissingName", "MissingEmail"),
                    CreateAccount("1", "OldName1", "OldEmail1"),
                    CreateAccount("3", "MatchName", "MatchEmail")
                });

            _mockGetNewAccounts
                .Setup(m => m.GetAll())
                .Returns(new List<Account> {
                    CreateAccount("1", "CorruptName1", "Email1"),
                    CreateAccount("2", "NewName", "NewEmail"),
                    CreateAccount("3", "MatchName", "MatchEmail")
                });

            var changes = _sut.GetChanges();
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.Missing));
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.New));
            Assert.AreEqual(1, changes.Count(c => c.ChangeType == ChangeType.Corrupted));
        }

        private Account CreateAccount(string id, string name, string email)
        {
            return new Account { Id = id, Name = name, Email = email };
        }
    }
}