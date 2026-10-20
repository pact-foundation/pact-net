#!/usr/bin/env bash
set -euo pipefail

FFI_VERSION="0.5.12"
FFI_BASE_URL="https://github.com/pact-foundation/pact-reference/releases/download/pact_ffi-v$FFI_VERSION"

GREEN="\e[32m"
YELLOW="\e[33m"
BLUE="\e[34m"
CLEAR="\e[0m"

base_path=$(dirname "$0")

download_native() {
    os="$1"
    platform="$2"
    target="$3"
    archive_ext="$4"
    lib="$5"

    # e.g.
    #   pact_ffi-x86_64-pc-windows-msvc.zip        (pact_ffi.dll at the archive root)
    #   pact_ffi-x86_64-unknown-linux-gnu.tar.xz   (pact_ffi-<target>/libpact_ffi.so)
    #   pact_ffi-aarch64-apple-darwin.tar.xz       (pact_ffi-<target>/libpact_ffi.dylib)
    src_archive="pact_ffi-$target.$archive_ext"
    src_sha="$src_archive.sha256"
    url="$FFI_BASE_URL/$src_archive"
    sha="$url.sha256"

    path="$base_path/$os/$platform"
    mkdir -p "$path"
    pushd "$path" > /dev/null

    echo -e "Downloading FFI library for ${YELLOW}$os/$platform${CLEAR}"
    echo -e "    Destination: ${BLUE}$path/$lib${CLEAR}"
    echo -e "    URL: ${BLUE}$url${CLEAR}"

    echo -n "    Downloading... "
    curl --silent --show-error --fail -L "$url" -o "$src_archive"
    curl --silent --show-error --fail -L "$sha" -o "$src_sha"
    echo -e "${GREEN}OK${CLEAR}"

    echo -n "    Verifying... "

    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed '/^$/d' "$src_sha" | shasum -a 256 --check --quiet -
    else
        sed '/^$/d' "$src_sha" | sha256sum --check --quiet -
    fi

    echo -e "${GREEN}OK${CLEAR}"

    echo -n "    Extracting... "
    extract_dir=$(mktemp -d)
    if [[ "$archive_ext" == "zip" ]]; then
        extract_zip "$src_archive" "$extract_dir"
        extracted="$extract_dir/$lib"
    else
        tar -xJf "$src_archive" -C "$extract_dir"
        extracted="$extract_dir/pact_ffi-$target/$lib"
    fi
    mv -f "$extracted" "$lib"
    rm -rf "$extract_dir" "$src_archive" "$src_sha"
    echo -e "${GREEN}OK${CLEAR}"
    echo ""

    popd > /dev/null
}

extract_zip() {
    if command -v unzip > /dev/null; then
        unzip -q -o "$1" -d "$2"
    elif command -v 7z > /dev/null; then
        7z x -y -o"$2" "$1" > /dev/null
    else
        tar -xf "$1" -C "$2"
    fi
}

download_native "windows" "x86_64" "x86_64-pc-windows-msvc" "zip" "pact_ffi.dll"
download_native "linux" "x86_64" "x86_64-unknown-linux-gnu" "tar.xz" "libpact_ffi.so"
download_native "linux" "aarch64" "aarch64-unknown-linux-gnu" "tar.xz" "libpact_ffi.so"
download_native "macos" "x86_64" "x86_64-apple-darwin" "tar.xz" "libpact_ffi.dylib"
download_native "macos" "aarch64" "aarch64-apple-darwin" "tar.xz" "libpact_ffi.dylib"

echo "Successfully downloaded FFI libraries"
