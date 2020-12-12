#! /bin/bash
output=$(git status)
if [[ "$output" == *"nothing to commit"* ]]; then
	echo "$(date) Already up-to-date" >> _GitLog.txt
else
	git add .
	git commit -m "refactoring"
	git push origin master
	echo "$(date) Updated" >> _GitLog.txt
fi

