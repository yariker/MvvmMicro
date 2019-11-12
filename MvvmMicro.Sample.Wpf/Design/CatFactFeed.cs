﻿using PlaidSoft.MvvmMicro.Sample.NetFx.Model;
using PlaidSoft.MvvmMicro.Sample.NetFx.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlaidSoft.MvvmMicro.Sample.NetFx.Design
{
    public class CatFactFeed : ICatFactFeed
    {
        public Task<Fact[]> GetFactsAsync(int amount, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new[]
            {
                new Fact
                {
                    PictureUri = new Uri("https://purr.objects-us-east-1.dream.io/i/vLgexh.jpg"),
                    Text = "A female cat will be pregnant for approximately 9 weeks - between 62 and 65 days from conception to delivery.",
                    UpdatedAt = DateTime.Now,
                },
                new Fact
                {
                    PictureUri = new Uri("https://purr.objects-us-east-1.dream.io/i/Eu8F6.jpg"),
                    Text = "It has been scientifically proven that stroking a cat can lower one's blood pressure.",
                    UpdatedAt = DateTime.Now,
                },
                new Fact
                {
                    PictureUri = new Uri("https://purr.objects-us-east-1.dream.io/i/hgyw6Tc.jpg"),
                    Text = "In an average year, cat owners in the United States spend over $2 billion on cat food.",
                    UpdatedAt = DateTime.Now,
                },
            });
        }
    }
}
