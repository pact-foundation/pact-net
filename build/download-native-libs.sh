#!/usr/bin/env bash
set -euo pipefail

FFI_VERSION="0.4.23"
FFI_BASE_URL="https://github.com/pact-foundation/pact-reference/releases/download/libpact_ffi-v$FFI_VERSION"

GREEN="\e[32m"
YELLOW="\e[33m"
BLUE="\e[34m"
CLEAR="\e[0m"

base_path=$(dirname "$0")

download_native() {
    file="$1"
    os="$2"
    platform="$3"
    extension="$4"

    # e.g.
    #   pact_ffi-windows-x86_64.dll.gz
    #   libpact_ffi-linux-x86_64.so.gz
    #   libpact_ffi-macos-x86_64.dylib.gz
    src_file="$file-$os-$platform.$extension"
    src_archive="$src_file.gz"
    src_sha="$src_archive.sha256"
    dest_file="$file.$extension"
    url="$FFI_BASE_URL/$src_archive"
    sha="$url.sha256"

    path="$base_path/$os/$platform"
    mkdir -p "$path"
    pushd $path > /dev/null

    echo -e "Downloading FFI library for ${YELLOW}$os/$platform${CLEAR}"
    echo -e "    Destination: ${BLUE}$path/$src_archive${CLEAR}"
    echo -e "    URL: ${BLUE}$url${CLEAR}"

    echo -n "    Downloading... "
    curl --silent -L "$url" -o "$src_archive"
    curl --silent -L "$sha" -o "$src_archive.sha256"
    echo -e "${GREEN}OK${CLEAR}"

    echo -n "    Verifying... "

    if [[ "$OSTYPE" == "darwin"* ]]; then
        shasum -a 256 --check --quiet "$src_sha"
    else
        if [[ "$OSTYPE" == "linux"* ]]; then
            if ldd /bin/ls >/dev/null 2>&1; then
                ldd_output=$(ldd /bin/ls)
                case "$ldd_output" in
                    *musl*) 
                        sha256sum -c -s "$src_sha"
                        ;;
                    *) 
                        sha256sum --check --quiet "$src_sha"
                        ;;
                esac
            else
                sha256sum --check --quiet "$src_sha"  
            fi
        else
            sha256sum --check --quiet "$src_sha"
        fi
    fi

    echo -e "${GREEN}OK${CLEAR}"

    echo -n "    Extracting... "
    gunzip -f "$src_archive"
    echo -e "${GREEN}OK${CLEAR}"
    echo ""

    mv "$src_file" "$dest_file"
    rm "$src_sha"

    popd > /dev/null
}

download_native "pact_ffi" "windows" "x86_64" "dll"
download_native "libpact_ffi" "linux" "x86_64" "so"
download_native "libpact_ffi" "linux" "aarch64" "so"
download_native "libpact_ffi" "macos" "x86_64" "dylib"
download_native "libpact_ffi" "macos" "aarch64" "dylib"
download_native "libpact_ffi" "linux" "x86_64-musl" "so"
download_native "libpact_ffi" "linux" "aarch64-musl" "so"

echo "Successfully downloaded FFI libraries"