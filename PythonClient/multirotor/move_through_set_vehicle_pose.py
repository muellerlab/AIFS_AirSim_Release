import setup_path 
import airsim
import time

client = airsim.VehicleClient()
client.confirmConnection()
pose1 = client.simGetVehiclePose()

pose1.orientation = airsim.to_quaternion(0.5, 0.5, 0.1)
client.simSetVehiclePose( pose1, False )

for i in range(3500):
    pose1 = client.simGetVehiclePose()
    pose1.position = pose1.position + airsim.Vector3r(0.03, 0, 0)
    pose1.orientation = pose1.orientation + airsim.to_quaternion(0.1, 0.1, 0.1)
    client.simSetVehiclePose( pose1, False )
    time.sleep(0.003)
    collision = client.simGetCollisionInfo()
    if collision.has_collided:
        print(collision)
