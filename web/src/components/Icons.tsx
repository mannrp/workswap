import React from 'react';

export interface IconProps extends React.SVGProps<SVGSVGElement> {
    size?: number;
}

export const Icons = {
    Calendar: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
    ),
    CheckCircle: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
    ),
    Swap: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
        </svg>
    ),
    Clock: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
    ),
    Plus: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M12 4v16m8-8H4" />
        </svg>
    ),
    List: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M4 7h16M4 12h8m-8 5h16" />
        </svg>
    ),
    Notification: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
        </svg>
    ),
    User: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
        </svg>
    ),
    Logout: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
        </svg>
    ),
    Settings: (props: IconProps) => (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width={props.size || 20} height={props.size || 20} {...props}>
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
    )
};
