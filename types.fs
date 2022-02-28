module types

type File =
    { Name: string
      Path: string
      Type: string
      Sha256: string }

type Count =
    { mutable Flac: int
      mutable Mp3: int
      mutable Others: int
      mutable SumAll: int }
