// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "common/common_utils/StrictMode.hpp"
STRICT_MODE_OFF
#ifndef RPCLIB_MSGPACK
#define RPCLIB_MSGPACK clmdep_msgpack
#endif // !RPCLIB_MSGPACK
#include "rpc/rpc_error.h"
STRICT_MODE_ON

#include "vehicles/multirotor/api/MultirotorRpcLibClient.hpp"
#include "common/common_utils/FileSystem.hpp"
#include <iostream>
#include <chrono>
#include <thread>



int main()
{
    using namespace msr::airlib;

    msr::airlib::MultirotorRpcLibClient client;
    typedef ImageCaptureBase::ImageRequest ImageRequest;
    typedef ImageCaptureBase::ImageResponse ImageResponse;
    typedef ImageCaptureBase::ImageType ImageType;
    typedef common_utils::FileSystem FileSystem;

    try {
        client.confirmConnection();

        std::cout << "Press Enter to get FPV image" << std::endl; std::cin.get();
        vector<ImageRequest> request = {ImageRequest("0", ImageType::DepthVis,false,true),ImageRequest("0", ImageType::DepthVis,false,true)};
        const vector<ImageResponse>& response = client.simGetImages(request);
        std::cout << "# of images received: " << response.size() << std::endl;

        if (response.size() > 0) {
            std::cout << "Enter path with ending separator to save images (leave empty for no save)" << std::endl;
            std::string path;
            path = "/home/clark/Documents/AirSim";
            // std::getline(std::cin, path);

            for (const ImageResponse& image_info : response) {
                std::cout << "Image uint8 size: " << image_info.image_data_uint8.size() << std::endl;
                std::cout << "Image float size: " << image_info.image_data_float.size() << std::endl;

                if (path != "") {
                    std::string file_path = FileSystem::combine(path, std::to_string(image_info.time_stamp));
                    if (image_info.pixels_as_float) {
                        Utils::writePFMfile(image_info.image_data_float.data(), image_info.width, image_info.height, file_path + ".pfm");
                    }
                    else {
                        std::ofstream file(file_path + ".png", std::ios::binary);
                        file.write(reinterpret_cast<const char*>(image_info.image_data_uint8.data()), image_info.image_data_uint8.size());
                        file.close();
                    }
                }
            }
        }
        

        // std::cout << "Press Enter to arm the drone" << std::endl; std::cin.get();
        // client.enableApiControl(true);
        // client.armDisarm(true);

        auto barometer_data = client.getBarometerData();
        std::cout << "Barometer data \n"
                  << "barometer_data.time_stamp \t" << barometer_data.time_stamp << std::endl
                  << "barometer_data.altitude \t" << barometer_data.altitude << std::endl
                  << "barometer_data.pressure \t" << barometer_data.pressure << std::endl
                  << "barometer_data.qnh \t" << barometer_data.qnh << std::endl;

        auto imu_data = client.getImuData();
        std::cout << "IMU data \n"
                  << "imu_data.time_stamp \t" << imu_data.time_stamp << std::endl
                  << "imu_data.orientation \t" << imu_data.orientation << std::endl
                  << "imu_data.angular_velocity \t" << imu_data.angular_velocity << std::endl
                  << "imu_data.linear_acceleration \t" << imu_data.linear_acceleration << std::endl;

        auto gps_data = client.getGpsData();
        std::cout << "GPS data \n"
                  << "gps_data.time_stamp \t" << gps_data.time_stamp << std::endl
                  << "gps_data.gnss.time_utc \t" << gps_data.gnss.time_utc << std::endl
                  << "gps_data.gnss.geo_point \t" << gps_data.gnss.geo_point << std::endl
                  << "gps_data.gnss.eph \t" << gps_data.gnss.eph << std::endl
                  << "gps_data.gnss.epv \t" << gps_data.gnss.epv << std::endl
                  << "gps_data.gnss.velocity \t" << gps_data.gnss.velocity << std::endl
                  << "gps_data.gnss.fix_type \t" << gps_data.gnss.fix_type << std::endl;

        auto magnetometer_data = client.getMagnetometerData();
        std::cout << "Magnetometer data \n"
                  << "magnetometer_data.time_stamp \t" << magnetometer_data.time_stamp << std::endl
                  << "magnetometer_data.magnetic_field_body \t" << magnetometer_data.magnetic_field_body << std::endl;
        // << "magnetometer_data.magnetic_field_covariance" << magnetometer_data.magnetic_field_covariance // not implemented in sensor

        std::cout << "Magnetometer data \n" 
            << "magnetometer_data.time_stamp \t" << magnetometer_data.time_stamp << std::endl 
            << "magnetometer_data.magnetic_field_body \t" << magnetometer_data.magnetic_field_body << std::endl; 
            // << "magnetometer_data.magnetic_field_covariance" << magnetometer_data.magnetic_field_covariance // not implemented in sensor

        // std::cout << "Press Enter to takeoff" << std::endl; std::cin.get();
        // float takeoffTimeout = 5; 
        // client.takeoffAsync(takeoffTimeout)->waitOnLastTask();

        // switch to explicit hover mode so that this is the fall back when
        // move* commands are finished.
        // std::this_thread::sleep_for(std::chrono::duration<double>(5));
        // client.hoverAsync()->waitOnLastTask();

        std::cout << "Press Enter to fly in a circular pattern" << std::endl; std::cin.get();
        // moveByVelocityZ is an offboard operation, so we need to set offboard mode.
        client.enableApiControl(true);

        auto position = client.getMultirotorState().getPosition();
        float z = position.z(); // current position (NED coordinate system).
        const float speed = 3.0f;
        const float size = 10.0f;
        const float duration = size / speed;

        auto orientation = client.getMultirotorState().getOrientation();

        float angle = 0;

        while (true){
            angle += 2*M_PI/(30);


            msr::airlib::Pose airsim_pose;
            airsim_pose.position[0] = position.x() + 5*cosf(angle);
            airsim_pose.position[1] = position.y() + 5*sinf(angle);
            airsim_pose.position[2] = position.z();

            airsim_pose.orientation.w() = orientation.w();
            airsim_pose.orientation.x() = orientation.x();
            airsim_pose.orientation.y() = orientation.y();
            airsim_pose.orientation.z() = orientation.z();
            client.simSetVehiclePose(airsim_pose, true);


            vector<ImageRequest> request = {ImageRequest("0", ImageType::DepthVis,false,true)};
            const vector<ImageResponse>& response = client.simGetImages(request);

            if (response.size() > 0) {
                std::string path;
                path = "/home/clark/Documents/AirSim";
                for (const ImageResponse& image_info : response) {
                    std::cout << "Image uint8 size: " << image_info.image_data_uint8.size() << std::endl;
                    std::cout << "Image float size: " << image_info.image_data_float.size() << std::endl;
                    std::string file_path = FileSystem::combine(path, std::to_string(image_info.time_stamp));
                    if (image_info.pixels_as_float) {
                        Utils::writePFMfile(image_info.image_data_float.data(), image_info.width, image_info.height,
                            file_path + ".pfm");
                    }
                    else {
                        std::ofstream file(file_path + ".png", std::ios::binary);
                        file.write(reinterpret_cast<const char*>(image_info.image_data_uint8.data()), image_info.image_data_uint8.size());
                        file.close();
                    }
                }
            }

        }

        // DrivetrainType driveTrain = DrivetrainType::ForwardOnly;
        // YawMode yaw_mode(true, 0);
        // std::cout << "moveByVelocityZ(" << speed << ", 0, " << z << "," << duration << ")" << std::endl;
        // client.moveByVelocityZAsync(speed, 0, z, duration, driveTrain, yaw_mode);
        // std::this_thread::sleep_for(std::chrono::duration<double>(duration));
        // std::cout << "moveByVelocityZ(0, " << speed << "," << z << "," << duration << ")" << std::endl;
        // client.moveByVelocityZAsync(0, speed, z, duration, driveTrain, yaw_mode);
        // std::this_thread::sleep_for(std::chrono::duration<double>(duration));
        // std::cout << "moveByVelocityZ(" << -speed << ", 0, " << z << "," << duration << ")" << std::endl;
        // client.moveByVelocityZAsync(-speed, 0, z, duration, driveTrain, yaw_mode);
        // std::this_thread::sleep_for(std::chrono::duration<double>(duration));
        // std::cout << "moveByVelocityZ(0, " << -speed << "," << z << "," << duration << ")" << std::endl;
        // client.moveByVelocityZAsync(0, -speed, z, duration, driveTrain, yaw_mode);
        // std::this_thread::sleep_for(std::chrono::duration<double>(duration));

        // client.hoverAsync()->waitOnLastTask();

        // std::cout << "Press Enter to land" << std::endl; std::cin.get();
        // client.landAsync()->waitOnLastTask();

        // std::cout << "Press Enter to disarm" << std::endl; std::cin.get();
        // client.armDisarm(false);

    }
    catch (rpc::rpc_error& e) {
        std::string msg = e.get_error().as<std::string>();
        std::cout << "Exception raised by the API, something went wrong." << std::endl
                  << msg << std::endl;
    }

    return 0;
}
