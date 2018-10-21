import os, sys, shutil
from os.path import abspath
from os.path import expanduser

def killItWithFire():
	if sys.platform == "Windows":
		appdata = os.getenv("LOCALAPPDATA")
		cache_dir = appdata + "\\Unity\\cache"
		shutil.rmtree(cache_dir)
	else:
		home = expanduser("~")
		cache_dir = home + "/Library/Unity/cache"
		shutil.rmtree(cache_dir)

try:
	killItWithFire()
except:
	pass