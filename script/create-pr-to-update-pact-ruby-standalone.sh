#!/usr/bin/env bash -e

: "${1?Please supply the pact-ruby-standalone version to upgrade to}"

STANDALONE_VERSION=$1
DASHERISED_VERSION=$(echo "${STANDALONE_VERSION}" | sed 's/\./\-/g')
BRANCH_NAME="chore/upgrade-to-pact-ruby-standalone-${DASHERISED_VERSION}"

git checkout master
git checkout standalone/install.ts
git pull origin master

git checkout -b ${BRANCH_NAME}

cat Build/Download-Standalone-Core.ps1 | sed "s/\$StandaloneCoreVersion = .*/\$StandaloneCoreVersion = '${STANDALONE_VERSION}';/" > tmp-Download-Standalone-Core.ps1
mv tmp-Download-Standalone-Core.ps1 Build/Download-Standalone-Core.ps1

git add Build/Download-Standalone-Core.ps1
git commit -m "feat(upgrade): update standalone to ${STANDALONE_VERSION}"
git push --set-upstream origin ${BRANCH_NAME}

hub pull-request --browse --message "feat: update standalone to ${STANDALONE_VERSION}"

git checkout master
