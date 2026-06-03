{{- define "fitlogs.hosts.blazorwebapp" -}}
{{- print "https://" (.Values.global.hosts.blazorwebapp | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
