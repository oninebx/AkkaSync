import React from 'react';

interface Props {
  color: string;
};

const StatusDot = (props: Props) => {
  return (
    <span className={`w-3 h-3 rounded-full ${props.color}`}></span>
  )
}

export default StatusDot;