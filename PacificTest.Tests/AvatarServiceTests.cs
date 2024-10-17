
using Microsoft.Extensions.Logging.Abstractions;
using PacificTest.Services;
using System.Net;
using System.Text.RegularExpressions;

namespace PacificTest.Tests
{
    public class AvatarServiceTests
    {

        [Fact]
        public async Task EndsWith1To5()
        {
            var avatarService = new AvatarService(new NullLogger<AvatarService>(), null);

            // act
            var url = await avatarService.GetAvatar("test1");

            // assert
            Assert.Equal("https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150", url);
        }

        [Fact]
        public async Task EndsWith6To9()
        {
            var client = new HttpClient();
            var avatarService = new AvatarService(new NullLogger<AvatarService>(), client);

            // act
            var url = await avatarService.GetAvatar("test6");

            // assert
            Assert.Equal("https://api.dicebear.com/8.x/pixel-art/png?seed=6&size=150", url);
        }

        [Fact]
        public async Task ContainsVowel()
        {
            var avatarService = new AvatarService(new NullLogger<AvatarService>(), null);

            // act
            var url = await avatarService.GetAvatar("test");

            // assert
            Assert.Equal("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150", url);
        }

        [Fact]
        public async Task ContainsNonAlphaNum()
        {
            var avatarService = new AvatarService(new NullLogger<AvatarService>(), null);

            // act
            var url = await avatarService.GetAvatar("111$$$");

            // assert
            Assert.Matches(new Regex(Regex.Escape("https://api.dicebear.com/8.x/pixel-art/png?seed=") + "[1-5]&size=150"), url);
        }

        [Fact]
        public async Task DefaultCatchAll()
        {
            var avatarService = new AvatarService(new NullLogger<AvatarService>(), null);

            // act
            var url = await avatarService.GetAvatar(null);

            // assert
            Assert.Equal(avatarService.GetDefaultAvatar(), url);
        }

    }
}