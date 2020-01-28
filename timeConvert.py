def timestamp2str(timestamp):
	ltime = time.localtime(timestamp)
	timeStr = time.strftime('%Y-%m-%d %H:%M:%S', ltime)
	return timeStr
