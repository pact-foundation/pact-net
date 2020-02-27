#!/usr/bin/env bash -e

: "${1?Please supply the pact-ruby-standalone version to upgrade to}"

STANDALONE_VERSION=$1
DASHERISED_VERSION=$(echo "${STANDALONE_VERSION}" | sed 's/\./\-/g')
BRANCH_NAME="chore/upgrade-to-pact-ruby-standalone-${DASHERISED_VERSION}"
VERSION_FILE="Build/Download-Standalone-Core.ps1"

git checkout master
git checkout ${VERSION_FILE}
git pull origin master

git checkout -b ${BRANCH_NAME}

cat ${VERSION_FILE} | sed "s/\$StandaloneCoreVersion = .*/\$StandaloneCoreVersion = '${STANDALONE_VERSION}';/" > tmp-version-file
mv tmp-version-file ${VERSION_FILE}

git add ${VERSION_FILE}
git commit -m "feat(upgrade): update standalone to ${STANDALONE_VERSION}"
git push --set-upstream origin ${BRANCH_NAME}

hub pull-request --browse --message "feat: update standalone to ${STANDALONE_VERSION}"

git checkout master
