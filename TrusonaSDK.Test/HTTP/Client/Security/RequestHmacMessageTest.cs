﻿//
// RequestHmacMessageTest.cs
//
// Author:
//       David Kopack <d@trusona.com>
//
// Copyright (c) 2018 Trusona, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Net.Http;
using System.Text;
using TrusonaSDK.HTTP;
using TrusonaSDK.HTTP.Client.Security;
using Xunit;
using FluentAssertions;

namespace TrusonaSDK.HTTP.Client.Security
{
  public class RequestHmacMessageTest
  {
    readonly HttpRequestMessage message;
    readonly IHmacMessage sut;

    public RequestHmacMessageTest()
    {
      message = new HttpRequestMessage()
      {
        RequestUri = new Uri("http://jones.net/bar?fizz=buzz"),
        Method = HttpMethod.Post,
        Content = new StringContent(
          content: @"{""foo"":""bar""}",
          mediaType: "application/json",
          encoding: Encoding.UTF8)
      };

      message.Headers.Add(Headers.X_DATE, "foobar");

      sut = new RequestHmacMessage(message);
    }

    [Fact]
    public void BodyDigest_should_return_the_md5_of_the_body_of_the_request()
    {
      sut.BodyDigest
         .Should()
         .Be("9bb58f26192e4ba00f01e2e7b136bbd8");
    }


    [Fact]
    public void BodyDigest_should_return_the_md5_of_empty_string_when_there_is_no_body()
    {
      message.Content = null;
      sut.BodyDigest
         .Should()
         .Be("d41d8cd98f00b204e9800998ecf8427e");
    }

    [Fact]
    public void ContentType_should_return_the_content_type_of_the_request()
    {
      sut.ContentType
         .Should()
         .Be("application/json; charset=utf-8");
    }

    [Fact]
    public void Date_should_return_the_value_of_the_date_header()
    {
      sut.Date
         .Should()
         .Be("foobar");
    }

    [Fact]
    public void Method_should_return_the_http_method_of_the_request()
    {
      sut.Method
         .Should()
         .Be("POST");
    }

    [Fact]
    public void RequestUri_should_return_the_uri_of_the_request()
    {
      sut.RequestUri
         .Should()
         .Be("/bar?fizz=buzz");
    }
  }
}