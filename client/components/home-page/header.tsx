"use client"

import React from 'react';
import {Button} from "@/components/ui/button";
import { useRouter } from "next/navigation"
import {ThemeToggle} from "@/components/theme-toggle";

const Header = () => {
    const router = useRouter();

    return (
        <div className="py-2 px-6 flex items-center justify-between z-90">
            <h1 className={"text-2xl font-bold"}>Studia</h1>

            <div className="flex items-center gap-1">
                <ThemeToggle />
                <Button
                    onClick={() => router.push("/login")}
                >
                    Sign Up
                </Button>
            </div>
        </div>
    );
};

export default Header;