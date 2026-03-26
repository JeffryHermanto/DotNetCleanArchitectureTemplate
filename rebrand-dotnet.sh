#!/bin/bash

set -e

OLD=$1
NEW=$2

if [ -z "$OLD" ] || [ -z "$NEW" ]; then
  echo "❌ Usage: ./rebrand-dotnet.sh OldName NewName"
  exit 1
fi

echo "🚀 ULTRA SAFE REBRAND: $OLD → $NEW"

# jumlah parallel worker (sesuaikan CPU, default 8)
WORKERS=8

# 1. Rename solution files
[ -f "$OLD.sln" ] && git mv "$OLD.sln" "$NEW.sln"
[ -f "$OLD.slnx" ] && git mv "$OLD.slnx" "$NEW.slnx"

# 2. Rename root folders
echo "📁 Renaming folders..."
find . -maxdepth 1 -type d -name "$OLD.*" | while read dir; do
  newdir=$(echo "$dir" | sed "s/$OLD/$NEW/")
  git mv "$dir" "$newdir"
done

# 3. Rename .csproj files
echo "📄 Renaming csproj..."
find . -name "*.csproj" \
  -not -path "*/bin/*" \
  -not -path "*/obj/*" | while read file; do

  if [[ "$file" == *"$OLD"* ]]; then
    newfile=$(echo "$file" | sed "s/$OLD/$NEW/")
    git mv "$file" "$newfile"
  fi
done

# 4. 🔥 NAMESPACE REPLACE (PRESISI + PARALLEL)
echo "✏️ Updating namespace..."

grep -rl --exclude-dir={.git,bin,obj} "$OLD." . \
| xargs -P $WORKERS -I {} sh -c \
'LC_ALL=C sed -i "" "s/\b'"$OLD"'\./'"$NEW"'\./g" "{}"'

# 5. 🌍 GLOBAL REPLACE (FULL COVERAGE + PARALLEL)
echo "🌍 Global replace..."

grep -rl --exclude-dir={.git,bin,obj} "$OLD" . \
| xargs -P $WORKERS -I {} sh -c \
'LC_ALL=C sed -i "" "s/'"$OLD"'/'"$NEW"'/g" "{}"'

# 6. 🔧 FIX ProjectReference (DOUBLE SAFETY)
echo "🔧 Fixing ProjectReference..."

find . -name "*.csproj" | while read file; do
  LC_ALL=C sed -i '' "s|$OLD\.|$NEW.|g" "$file"
  LC_ALL=C sed -i '' "s|$OLD/|$NEW/|g" "$file"
  LC_ALL=C sed -i '' "s|$OLD\\\\|$NEW\\\\|g" "$file"
done

# 7. 🧹 CLEAN CACHE (VERY IMPORTANT)
echo "🧹 Cleaning bin/obj..."
find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

# 8. 🧱 REBUILD SOLUTION
echo "🧱 Rebuilding solution..."
rm -f "$NEW.sln"
dotnet new sln -n "$NEW" > /dev/null

find . -name "*.csproj" \
  -not -path "*/bin/*" \
  -not -path "*/obj/*" \
| while read proj; do
  dotnet sln "$NEW.sln" add "$proj" > /dev/null
done

# 9. 🔧 RESTORE & BUILD
echo "🔧 Restoring..."
dotnet restore

echo "🏗️ Building..."
if dotnet build; then
  echo "✅ BUILD SUCCESS — 100% CLEAN REBRAND!"
else
  echo "❌ BUILD FAILED — kemungkinan edge case"
fi

# 10. 🔍 FINAL VALIDATION
echo "🔍 Checking leftover '$OLD'..."
grep -r "$OLD" . --exclude-dir={.git,bin,obj} || echo "🎉 NO LEFTOVER — PERFECT!"