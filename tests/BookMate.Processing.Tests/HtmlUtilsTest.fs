namespace BookMate.Processing.Tests

module HtmlUtilsTest = 
  open System
  open System.IO
  open Xunit
  open FsUnit.Xunit
  open BookMate.Processing
  open BookMate.Processing.HtmlUtils
  
  let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
  let sampleHtmlDoc = Path.Combine(sampleDirectory, "epub30-titlepage.xhtml")
  let sampleTxtDoc = Path.Combine(sampleDirectory, "epub30-titlepage.txt")
  
  [<Fact>]
  let ``Should read all text from *HTML file``() = 
      let fileText = sampleHtmlDoc |> File.ReadAllText
      let html = loadHtml fileText
      let actualText = getTextFromHtml html
      let expectedText = sampleTxtDoc |> File.ReadAllLines
      actualText |> should equal expectedText
  
  [<Fact>]
  let ``Cloned *HTML document should be exactly the same``() = 
      let fileText = sampleHtmlDoc |> File.ReadAllText
      let html = loadHtml fileText
      let clonedHtml = cloneHtmlDocument html
      html.CheckSum |> should equal clonedHtml.CheckSum
      html.DeclaredEncoding |> should equal clonedHtml.DeclaredEncoding 
      html.Encoding |> should equal clonedHtml.Encoding
      clonedHtml.DocumentNode.OuterHtml |> should equal html.DocumentNode.OuterHtml
      clonedHtml.DocumentNode.InnerHtml |> should equal html.DocumentNode.InnerHtml
      clonedHtml.DocumentNode.InnerText |> should equal html.DocumentNode.InnerText

  [<Fact>]
  let ``Should not alter document structure`` ()=
    let fileText = sampleHtmlDoc |> File.ReadAllText
    let html = loadHtml fileText
    let processedHtml = processNodes html (id)

    processedHtml.DocumentNode.OuterHtml |> should equal html.DocumentNode.OuterHtml
    processedHtml.DocumentNode.InnerHtml |> should equal html.DocumentNode.InnerHtml
    processedHtml.DocumentNode.InnerText |> should equal html.DocumentNode.InnerText