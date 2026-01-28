"use client"

import React from 'react';
import Image from "next/image";
import banner from "@/public/banner.png"

const MainBlock = () => {
    return (
        <div className="h-full flex flex-row gap-4 items-start justify-center w-full">
            <div className="mt-26 ml-16 flex flex-col items-start justify-center w-full">
                <h1 className={"flex flex-col text-5xl"}>
                    <span className={"text-6xl font-bold"}>Welcome</span>
                    to Studia educational online platform
                </h1>
                <h2 className={"mt-2 text-muted-foreground"}>Learn course, create you own, add teacher, students and go on! </h2>
            </div>
            <div className=" mt-0 mr-10 relative h-full flex justify-center items-center">
                {/*<div*/}
                {/*    className={`*/}
                {/*      absolute*/}
                {/*      w-180 h-180*/}
                {/*      rounded-full*/}
                {/*      bg-[radial-gradient(circle,theme(colors.primary.DEFAULT)_0%,transparent_65%)]*/}
                {/*      blur-2xl*/}
                {/*      translate-x-25*/}
                {/*      -translate-y-10*/}
                {/*    `}*/}
                {/*/>*/}

                <Image
                    src={banner}
                    alt="banner"
                    className="relative z-10"
                />
            </div>
        </div>
    );
};

export default MainBlock;