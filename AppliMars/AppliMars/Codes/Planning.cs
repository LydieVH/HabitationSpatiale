﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Planning
{
    public virtual List<Journee> tableauJournees {
        get;
        set;
    }

	public virtual int _compteJour
	{
		get;
		set;
	}

	public virtual string _nomPlanning
	{
		get;
		set;
	}

	public virtual void majCompteur()
	{
		throw new System.NotImplementedException();
	}

	public Planning()
	{
	}

}

