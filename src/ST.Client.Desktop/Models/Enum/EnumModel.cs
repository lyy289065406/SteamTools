﻿using ReactiveUI;
using System;
using System.Application.UI.Resx;
using System.Collections.Generic;
using System.Text;

namespace System.Application.Models
{
    public class EnumModel : ReactiveObject
    {
        public static List<EnumModel<E>> GetEnumModels<E>() where E : struct, Enum
        {
            var list = new List<EnumModel<E>>();
            foreach (var key in Enum.GetNames(typeof(E)))
            {
                list.Add(new EnumModel<E>(key, Enum.Parse<E>(key)));
            }
            return list;
        }
    }

    public class EnumModel<T> : ReactiveObject where T : struct, Enum
    {
        public EnumModel() { }

        public EnumModel(string name, T value)
        {
            Name = name;
            Value = value;
        }

        private string? _Name;
        public string? Name
        {
            get => _Name;
            set => this.RaiseAndSetIfChanged(ref _Name, value);
        }

        public string? Description
        {
            get => Enum2.GetDescription<T>(Value);
        }

        public string? Name_Localiza
        {
            get => string.IsNullOrEmpty(Description)
                ? AppResources.ResourceManager.GetString(Name, AppResources.Culture)
                : AppResources.ResourceManager.GetString(Description, AppResources.Culture);
        }

        private T _Value;
        public T Value
        {
            get => _Value;
            set => this.RaiseAndSetIfChanged(ref _Value, value);
        }

        private bool _Enable;
        public bool Enable
        {
            get => _Enable;
            set => this.RaiseAndSetIfChanged(ref _Enable, value);
        }


        /// <summary>
        /// 枚举作为type时统计集合数量
        /// </summary>
        private int _Count;
        public int Count
        {
            get => _Count;
            set => this.RaiseAndSetIfChanged(ref _Count, value);
        }
    }
}
