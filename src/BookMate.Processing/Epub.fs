namespace BookMate.Processing

module Epub =
    type EpubFilePath = EpubFilePath of string
    type UnpackedDirPath = UnpackedDirPath of string
    type PackDirPath = PackDirPath of string
    type UnpackedPath = UnpackedPath of EpubFilePath * UnpackedDirPath