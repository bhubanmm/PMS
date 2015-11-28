# Standard Imports
import urllib
import urllib2

# Set the URL
url = "http://pmsdata.azurewebsites.net/odata/Parking/"

# Prepare data for POST operation
data = urllib.urlencode({'ParkingID':'2','SlotsOccupied':'12'})

# Call the service
urllib2.urlopen(url, data).read()
