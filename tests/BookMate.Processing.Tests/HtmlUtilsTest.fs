namespace BookMate.Processing.Tests

module HtmlUtilsTest = 
  open System
  open System.IO
  open Xunit
  open FsUnit.Xunit
  open BookMate.Processing
  
  let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
  let sampleHtmlDoc = Path.Combine(sampleDirectory, "epub30-titlepage.xhtml")
  let sampleTxtDoc = Path.Combine(sampleDirectory, "epub30-titlepage.txt")
  
  [<Fact>]
  let ``Should read all text from *HTML file``() = 
      let fileText = sampleHtmlDoc |> File.ReadAllText
      let html = HtmlUtils.loadHtml fileText
      let actualText = HtmlUtils.getTextFromHtml html
      let expectedText = sampleTxtDoc |> File.ReadAllLines
      actualText = expectedText |> should be True
  
  [<Fact>]
  let ``Cloned *HTML document should be exactly the same``() = 
      let fileText = sampleHtmlDoc |> File.ReadAllText
      let html = HtmlUtils.loadHtml fileText
      let clonedHtml = HtmlUtils.cloneHtmlDocument html
      html.CheckSum = clonedHtml.CheckSum |> should be True
      html.DeclaredEncoding = clonedHtml.DeclaredEncoding |> should be True
      html.Encoding = clonedHtml.Encoding |> should be True
