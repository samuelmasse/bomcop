# bomcop

This is simple tool you can run on a directory that will recursively find all files that do not have a UTF-8 BOM

## Example

Running bomcop from command line on this repo will reveal that `action.yml` is not encoded with a UTF-8 BOM

```
$ bomcop
action.yml:
    UTF-8 BOM missing at start of file

1 bom missing
```

## Usage

```
$ bomcop --help
bomcop 1.0.0
Copyright (C) 2022 bomcop

  --exclude       List of file regex patterns to be excluded

  --no-color      Turn off colors for output

  --help          Display this help screen.

  --version       Display version information.

  dir (pos. 0)    Directory to check for BOM
```

## Excludes

These file patterns are excluded by default from the search. If the directory is in a git repo, files ignored by `.gitignore` will also be ignored by this tool.

```
**/.vscode/*.json
**/*.dll
**/*.snap
**/*.otf
**/*.woff
**/*.eot
**/*.ttf
**/*.gif
**/*.png
**/*.jpg
**/*.jpeg
**/*.webp
**/*.avif
**/*.mp4
**/*.wmv
**/*.svg
**/*.ico
**/*.bak
**/*.bin
**/*.pdf
**/*.zip
**/*.gz
**/*.tar
**/*.7z
**/*.bz2
**/*.log
**/*.patch
```

You can add more using the `--exclude` option

```
bomcop --exclude **/package-lock.json **/yarn.lock
```

## Github Action

Create a `.github/workflows/bom.yml` file with this template to run bomcop automatically on your codebase

```yml
name: Run bomcop
on:
  push:
  pull_request:
jobs:
  build:
    name: Run bomcop
    runs-on: ubuntu-latest
    steps:
      - name: Checkout Code
        uses: actions/checkout@v3
        with:
          fetch-depth: 0
      - name: Run bomcop
        uses: samuelmasse/bomcop@master
        with:
          # exclude: "any file patterns you want to exclude"
```
