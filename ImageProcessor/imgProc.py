# Standard Imports
import cv2

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

