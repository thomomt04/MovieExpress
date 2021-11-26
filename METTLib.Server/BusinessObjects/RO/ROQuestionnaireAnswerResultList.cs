﻿// Generated 16 Jul 2018 10:27 - Singular Systems Object Generator Version 2.2.694
//<auto-generated/>
using System;
using Csla;
using Csla.Serialization;
using Csla.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Singular;
using System.Data;
using System.Data.SqlClient;


namespace METTLib.RO
{
	[Serializable]
	public class ROQuestionnaireAnswerResultList
	 : METTReadOnlyListBase<ROQuestionnaireAnswerResultList, ROQuestionnaireAnswerResult>
	{
		#region " Parent "

		[NotUndoable()] private ROQuestionnaireGroupAnswerResult mParent;
		#endregion

		#region " Business Methods "

		public ROQuestionnaireAnswerResult GetItem(int QuestionnaireAnswerResultID)
		{
			foreach (ROQuestionnaireAnswerResult child in this)
			{
				if (child.QuestionnaireAnswerResultID == QuestionnaireAnswerResultID)
				{
					return child;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "Questionnaire Answer Results";
		}

		#endregion

		#region " Data Access "

		public static ROQuestionnaireAnswerResultList NewROQuestionnaireAnswerResultList()
		{
			return new ROQuestionnaireAnswerResultList();
		}

		public ROQuestionnaireAnswerResultList()
		{
			// must have parameter-less constructor
		}

		#endregion

	}

}