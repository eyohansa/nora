﻿using log4net;
using nora.lara.unpackers;
using System;
using System.Collections.Generic;

namespace nora.lara.state {

    public abstract class Property {

        public static Property For(PropertyInfo info) {
            switch (info.Type) {
                case PropertyInfo.PropertyType.Int: return new IntProperty(info);
                case PropertyInfo.PropertyType.Float: return new FloatProperty(info);
                case PropertyInfo.PropertyType.Vector: return new VectorProperty(info);
                case PropertyInfo.PropertyType.VectorXy: return new VectorXyProperty(info);
                case PropertyInfo.PropertyType.String: return new StringProperty(info);
                case PropertyInfo.PropertyType.Array: return new ArrayProperty(info);
                case PropertyInfo.PropertyType.Int64: return new Int64Property(info);
                default: throw new InvalidOperationException();
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(Property));

        public readonly PropertyInfo Info;
        public uint UpdatedAt { get; private set; }

        protected Property(PropertyInfo info) {
            this.Info = info;
        }

        public abstract Property Copy();

        protected abstract void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream);

        public void Update(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
            this.UpdatedAt = tick;
            Unpack(tick, unpacker, stream);
        }

        public T ValueAs<T>() {
            return ((TypedProperty<T>) this).Value;
        }

        public abstract class TypedProperty<T> : Property {

            public T Value { get; protected set; }

            protected TypedProperty(PropertyInfo info) : base(info) {
            }

            public override string ToString() {
                return this.Value.ToString();
            }
        }

        public class IntProperty : TypedProperty<uint> {

            public IntProperty(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new IntProperty(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackInt(Info, stream);
            }
        }

        public class FloatProperty : TypedProperty<float> {

            public FloatProperty(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new FloatProperty(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackFloat(Info, stream);
            }
        }

        public class VectorProperty : TypedProperty<Vector> {
            
            public VectorProperty(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new VectorProperty(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackVector(Info, stream);
            }
        }

        public class VectorXyProperty : TypedProperty<VectorXy> {
            
            public VectorXyProperty(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new VectorXyProperty(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackVectorXy(Info, stream);
            }
        }

        public class StringProperty : TypedProperty<string> {

            public StringProperty(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new StringProperty(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackString(Info, stream);
            }
        }

        public class ArrayProperty : TypedProperty<List<Property>> {

            public ArrayProperty(PropertyInfo info) : base(info) {
                this.Value = new List<Property>();
            }

            public override Property Copy() {
                var copy = new ArrayProperty(Info);

                foreach (var item in this.Value) {
                    copy.Value.Add(item.Copy());
                }

                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                unpacker.UnpackArray(tick, Value, Info, stream);
            }
        }

        public class Int64Property : TypedProperty<ulong> {

            public Int64Property(PropertyInfo info) : base(info) {
            }

            public override Property Copy() {
                var copy = new Int64Property(Info);
                copy.Value = Value;
                return copy;
            }

            protected override void Unpack(uint tick, PropertyValueUnpacker unpacker, Bitstream stream) {
                Value = unpacker.UnpackInt64(Info, stream);
            }
        }
    }
}
