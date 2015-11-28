# Standard Imports
import cv2
import threading
import time
import urllib
import urllib2

# Create demon threads to execute image processor every 10 seconds
thrd = threading.Timer(10.0, processImage)
thrd.daemon = True
thrd.start()

# Keep the main thread alive
while True:
    time.sleep(1)

def processImage():
    # Read the reference Image
    refImg = cv2.imread("/home/pi/cam/base.jpg")

    # Read the new image
    outImg = cv2.imread("/home/pi/cam/test.jpg")

    # Create the Background Subtractor Object
    fgbg = cv2.BackgroundSubtractorMOG()

    # Apply the reference image to the Background Subtractor object
    fgmask = fgbg.apply(refImg)

    # Apply the new image to the Background Subtractor object to get the mask
    fgmask = fgbg.apply(outImg)

    # Save the differential image
    cv2.imwrite("/home/pi/cam/difImg.jpg", fgmask)

    # Setup SimpleBlobDetector parameters.
    params = cv2.SimpleBlobDetector_Params()

    # Change thresholds
    params.minThreshold = 10
    params.maxThreshold = 200

    # Filter by Area.
    params.filterByArea = True
    params.minArea = 35

    # Filter by Circularity
    params.filterByCircularity = True
    params.minCircularity = 0.1

    # Filter by Convexity
    params.filterByConvexity = True
    params.minConvexity = 0.87
        
    # Filter by Inertia
    params.filterByInertia = True
    params.minInertiaRatio = 0.01

    # Create a detector with the parameters
    blobDetector = cv2.SimpleBlobDetector(params)

    # Detect blobs.
    keypoints = blobDetector.detect(difImg)
    print "Number of Objects Detected: {0}".format(len(keypoints))

    # Set the URL
    url = "http://pmsdata.azurewebsites.net/odata/Parking/"

    # Prepare data for POST operation
    data = urllib.urlencode({'ParkingID':'2','SlotsOccupied':'{0}'.format(len(keypoints))})

    # Call the service
    urllib2.urlopen(url, data).read()
